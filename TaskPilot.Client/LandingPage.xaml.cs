using Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows.Input;
using TaskPilot.Client.Services;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaskPilot.Client;

public partial class LandingPage : ContentPage, INotifyPropertyChanged
{
    // --- Pomodoro Timer Fields ---
    private const int PomodoroDurationSeconds = 25 * 60; // 25 minutes
    private int _remainingSeconds;
    private IDispatcherTimer _timer;
    private bool _isTimerRunning;
    private int _secondsElapsedInSession = 0;
    // -----------------------------

    private const double CompletedPriorityValue = 10.0;

    private TodoGetDto _selectedTodo;

    public string TimerButtonColor => EnableTimer ? "#1A1D3F" : "Gray";

    private bool _enableTimer;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public List<TodoGetDto> allTodos { get; set; } = new();
    public ObservableCollection<TodoGetDto> listOfTodos { get; set; } = new();
    private bool _isShowingAll = false;
    public string ViewAllButtonText => _isShowingAll ? "Show less" : "View all";
    private readonly HttpClient _httpClient = new HttpClient();
    public string storedID = "";

    private readonly TodoService _todoService;

    public bool EnableTimer
    {
        get => _enableTimer;
        set
        {
            if (_enableTimer != value)
            {
                _enableTimer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimerButtonColor));
            }
        }
    }
    public bool IsNavigationEnabled => !_isTimerRunning;

    private void OnPropertyChangedForTimerState()
    {
        OnPropertyChanged(nameof(IsNavigationEnabled));
        OnPropertyChanged(nameof(EnableTimer));
        OnPropertyChanged(nameof(TimerButtonColor));

        // Swap images based on navigation state
        if (IsNavigationEnabled)
        {
            TodoImageSource = "todo.jpg";
            CalendarImageSource = "calendar.jpg";
            ProfileImageSource = "profile.jpg";
        }
        else
        {
            TodoImageSource = "todo_gray.png";
            CalendarImageSource = "calendar_gray.png";
            ProfileImageSource = "profile_gray.png";
        }
    }

    public ICommand ViewAllProjectsCommand { get; }

    // Guard to prevent concurrent toggle requests
    private bool _isToggleInProgress;

    private string _todoImageSource = "todo.jpg";
    private string _calendarImageSource = "calendar.jpg";
    private string _profileImageSource = "profile.jpg";

    public string TodoImageSource
    {
        get => _todoImageSource;
        set { _todoImageSource = value; OnPropertyChanged(); }
    }
    public string CalendarImageSource
    {
        get => _calendarImageSource;
        set { _calendarImageSource = value; OnPropertyChanged(); }
    }
    public string ProfileImageSource
    {
        get => _profileImageSource;
        set { _profileImageSource = value; OnPropertyChanged(); }
    }

    public LandingPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Config.BaseUrl)
        };

        // Initialize the TodoService with the same HttpClient so client
        _todoService = new TodoService(_httpClient);

        ViewAllProjectsCommand = new Command(ToggleViewAllProjects);


        // --- Initialize Pomodoro Timer ---
        InitializePomodoroTimer();
        UpdateTimerDisplay();
        // ---------------------------------

        EnableTimer = false;

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        storedID = Preferences.Get("UserID", null);

        LabelStudentName.Text = Preferences.Get("StudentName", "Name")
                              + " "
                              + Preferences.Get("StudentSurname", "Surname");

        _isShowingAll = false;

        allTodos.Clear();

        if (int.TryParse(storedID, out var id))
        {
            await GetTodos(id);
        }
    }

    // --- Pomodoro Timer Methods ---
    private enum PomodoroState
    {
        Work,
        ShortBreak,
        LongBreak
    }

    private PomodoroState _currentState = PomodoroState.Work;
    private int _pomodoroCount = 0; // Number of completed work sessions in this cycle
    private int _workSecondsAccumulated = 0; // Total seconds spent in work sessions since last reset

    private const int ShortBreakSeconds = 5 * 60;
    private const int LongBreakSeconds = 15 * 60;

    private void InitializePomodoroTimer()
    {
        _remainingSeconds = PomodoroDurationSeconds;
        _isTimerRunning = false;
        _currentState = PomodoroState.Work;
        _pomodoroCount = 0;
        _workSecondsAccumulated = 0;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        _remainingSeconds--;
        _secondsElapsedInSession++;

        UpdateTimerDisplay();

        if (_remainingSeconds <= 0)
        {
            _timer.Stop();
            _isTimerRunning = false;

            if (_currentState == PomodoroState.Work)
            {
                // Accumulate work session time
                _workSecondsAccumulated += PomodoroDurationSeconds;
                _pomodoroCount++;

                if (_pomodoroCount % 4 == 0)
                {
                    // Long break after 4 work sessions
                    _currentState = PomodoroState.LongBreak;
                    _remainingSeconds = LongBreakSeconds;
                    await DisplayAlertAsync("Long Break", "Take a 15 minute break!", "OK");
                }
                else
                {
                    // Short break
                    _currentState = PomodoroState.ShortBreak;
                    _remainingSeconds = ShortBreakSeconds;
                    await DisplayAlertAsync("Short Break", "Take a 5 minute break!", "OK");
                }
            }
            else // Break finished
            {
                _currentState = PomodoroState.Work;
                _remainingSeconds = PomodoroDurationSeconds;
                await DisplayAlertAsync("Work Session", "Back to work!", "OK");
            }

            _secondsElapsedInSession = 0;
            StartStopButton.Text = "Stop Pomodoro"; // Keep as "Stop" since timer can continue
            UpdateTimerDisplay();

            // Automatically start the next session (work or break)
            _timer.Start();
            _isTimerRunning = true;
            OnPropertyChangedForTimerState();
        }
    }

    // Connects to the button in MainPage.xaml
    private async void OnStartStopButtonClicked(object sender, EventArgs e)
    {
        if (_isTimerRunning)
        {
            // Stopping mid-session or during a break: add all accumulated work time to the todo
            _timer.Stop();
            _isTimerRunning = false;

            // If currently in a work session, add the elapsed time in this session
            if (_currentState == PomodoroState.Work)
            {
                _workSecondsAccumulated += _secondsElapsedInSession;
            }

            // Add all accumulated work time to the todo
            await UpdateTaskTimeSpent(_workSecondsAccumulated / 60);

            // Reset all state
            _remainingSeconds = PomodoroDurationSeconds;
            _secondsElapsedInSession = 0;
            _workSecondsAccumulated = 0;
            _pomodoroCount = 0;
            _currentState = PomodoroState.Work;

            UpdateTimerDisplay();
            StartStopButton.Text = "Start Pomodoro";
            OnPropertyChangedForTimerState();
        }
        else
        {
            _timer.Start();
            _isTimerRunning = true;
            StartStopButton.Text = "Stop Pomodoro";
            OnPropertyChangedForTimerState();
        }
    }

    // Update to accept minutes as parameter
    private async Task UpdateTaskTimeSpent(int minutesToAdd)
    {
        if (_selectedTodo == null || minutesToAdd <= 0)
            return;

        try
        {
            bool success = await _todoService.UpdateTimeSpentAsync(_selectedTodo.Id, minutesToAdd);

            if (success)
            {
                if (int.TryParse(storedID, out var id))
                {
                    await GetTodos(id); // reload from server
                }
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to update time spent.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to update time spent: {ex.Message}", "OK");
        }
    }

    private void UpdateTimerDisplay()
    {
        // Convert seconds to MM:SS format
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;

        // Use :D2 format specifier to ensure leading zeros (e.g., 05:03)
        TimerDisplayLabel.Text = $"{minutes:D2}:{seconds:D2}";
    }
    // ------------------------------

    public void TodoRedirect()
    {
        Shell.Current.GoToAsync("//TodoPage");
    }

    private void ToggleViewAllProjects()
    {
        _isShowingAll = !_isShowingAll; // Flip the state
        UpdateDisplayedTodos();         // Update the collection
        OnPropertyChanged(nameof(ViewAllButtonText)); // Notify the UI that the button text has changed
    }
    public async Task GetTodos(int id)
    {
        try
        {
            // Call the API to get todos for the student
            var response = await _httpClient.PostAsJsonAsync("api/Todo/GetTodos", id);

            // If the server returns an error, throw it so the UI can display it.
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the list of todos
                var todos = await response.Content.ReadFromJsonAsync<List<TodoGetDto>>();

                // Keep the full list if you need it elsewhere
                allTodos = todos ?? new List<TodoGetDto>();

                // Assign ToggleCompleteCommand for each todo
                foreach (var todo in allTodos)
                {
                    todo.PropertyChanged -= Todo_PropertyChanged; // Prevent double subscription
                    todo.PropertyChanged += Todo_PropertyChanged;
                }

                // Now update the UI Based on the flag state (view all) or (view top 4)
                UpdateDisplayedTodos();

            }

        }
        catch (Exception)
        {

            // default template
        }
    }

    private async Task ToggleTodoCompletion(TodoGetDto todo)
    {
        if (todo == null) return;

        // prevent double taps/race conditions
        if (_isToggleInProgress) return;
        _isToggleInProgress = true;

        var previousIsCompleted = todo.IsCompleted;
        var previousPrioritySelection = todo.PrioritySelection;

        try
        {
            // Only update UI after backend confirms
            bool success = await _todoService.ToggleCompletion(todo.Id);

            if (success)
            {
                
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to mark task as completed.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to update task status: {ex.Message}", "OK");
        }
        finally
        {
            _isToggleInProgress = false;
        }
    }

    private void UpdateDisplayedTodos()
    {
        listOfTodos.Clear();

        var sortedTodos = allTodos.OrderBy(t => t.PrioritySelection); // Lower number is higher priority

        var todosToShow = _isShowingAll ? sortedTodos : sortedTodos.Take(4);

        foreach (var todo in todosToShow)
        {
            listOfTodos.Add(todo);
        }
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is TodoGetDto task)
        {
            // Use triple-slash absolute route to reset navigation stack and navigate to TodoPage
            await Shell.Current.GoToAsync("///TodoPage", true, new Dictionary<string, object>
        {
            { "TaskToEdit", task }
        });
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_selectedTodo != null)
        {
            _selectedTodo.IsSelected = false;

            _selectedTodo = null;
        }
        EnableTimer = false;
    }

    public async void OnTodoPageClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//TodoPage");
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is TodoGetDto tappedDto)
        {
            // Prevent changing selection while timer is running and a todo is already selected
            if (_isTimerRunning && _selectedTodo != null && _selectedTodo != tappedDto)
            {
                return; // Ignore selection change
            }

            if (_selectedTodo != null && _selectedTodo == tappedDto)
            {
                if (_isTimerRunning)
                    return; // Prevent deselection while timer is running

                tappedDto.IsSelected = false;
                EnableTimer = false;
                _selectedTodo = null;
            }
            else
            {
                if (_selectedTodo != null)
                    _selectedTodo.IsSelected = false;

                tappedDto.IsSelected = true;
                EnableTimer = true;
                _selectedTodo = tappedDto;
            }
        }
    }

    public async void OnProfilePageClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ProfilePage");
    }

    public async void OnCalendarPageClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//CalendarPage");
    }

    private async void Todo_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodoGetDto.IsCompleted) && sender is TodoGetDto todo)
        {
            // Call your backend to update completion status
            await ToggleTodoCompletion(todo);

            if (int.TryParse(storedID, out var id))
            {
                await GetTodos(id); // reload from server
            }
            else
            {
                UpdateDisplayedTodos();
            }
        }
    }
}