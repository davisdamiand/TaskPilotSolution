using Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows.Input;
using TaskPilot.Client.Services;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaskPilot.Client;

public partial class MainPage : ContentPage, INotifyPropertyChanged
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

    public ICommand ViewAllProjectsCommand { get; }

    // Guard to prevent concurrent toggle requests
    private bool _isToggleInProgress;

    public MainPage()
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
    private void InitializePomodoroTimer()
    {
        _remainingSeconds = PomodoroDurationSeconds;
        _isTimerRunning = false;

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
            // Timer finished
            _timer.Stop();
            _isTimerRunning = false;

            await UpdateTaskTimeSpent();
            _secondsElapsedInSession = 0; // Reset for next session

            // Update button text to reflect a new session is ready
            StartStopButton.Text = "Start Pomodoro";

            // Display an alert for the break
            await DisplayAlertAsync("Pomodoro Finished!", "Time for a short break!", "OK");
            UpdateTimerDisplay();
        }

        
    }

    // Connects to the button in MainPage.xaml
    private async void OnStartStopButtonClicked(object sender, EventArgs e)
    {
        if (_isTimerRunning)
        {
            // Stop Timer
            _timer.Stop();
            _isTimerRunning = false;

            await UpdateTaskTimeSpent();

            _remainingSeconds = PomodoroDurationSeconds;
            _secondsElapsedInSession = 0;

            UpdateTimerDisplay();
            StartStopButton.Text = "Start Pomodoro";
        }
        else
        {
            _timer.Start();
            _isTimerRunning = true;
            StartStopButton.Text = "Stop Pomodoro";
        }
    }

    private async Task UpdateTaskTimeSpent()
    {
        //Check 
        if (_selectedTodo == null)
        {
            return; // Nothing to log
        }

        int minutesToAdd = _secondsElapsedInSession / 60;

        try
        {
            bool success = await _todoService.UpdateTimeSpentAsync(_selectedTodo.Id, minutesToAdd);


            if (success)
            {
                // Update local state to reflect the change
                _selectedTodo.TimeSpentMinutes += minutesToAdd;

                Console.WriteLine($"{minutesToAdd} minute(s) successfully logged for task '{_selectedTodo.Title}'.");
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
            var response = await _httpClient.PostAsJsonAsync("api/Todo/GetTodos", id);

            if (response.IsSuccessStatusCode)
            {
                var todos = await response.Content.ReadFromJsonAsync<List<TodoGetDto>>();

                // Keep the full list if you need it elsewhere
                allTodos = todos ?? new List<TodoGetDto>();

                foreach (var todo in allTodos)
                {
                    todo.ToggleCompleteCommand = new Command(async () => await ToggleTodoCompletion(todo));
                }

                // Now update the UI Based on the flag state (view all) or (view top 4)
                UpdateDisplayedTodos();

            }

        }
        catch (Exception)
        {

            //Do nothing
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
            // local update so UI + ViewAll state are immediately consistent
            todo.IsCompleted = true;
            todo.PrioritySelection = CompletedPriorityValue;

            // 1. Call your service to update the database
            bool success = await _todoService.ToggleCompletion(todo.Id);

            // 2. If the server confirms, re-sync from server to ensure UI reflects server state
            if (success)
            {
                if (int.TryParse(storedID, out var id))
                {
                    await GetTodos(id); // server approach: reload the todo list
                }
                else
                {
                    // fallback: update local state if storedID is missing
                    await DisplayAlertAsync("Error", "Failed to mark task as completed.", "OK");
                }
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
            await Shell.Current.GoToAsync(nameof(TodoPage), true, new Dictionary<string, object>
        {
            { "TaskToEdit", task }
        });
        }
    }

    public async void OnTodoPageClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//TodoPage");
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        // Get the data context of the tapped item
        if ((sender as BindableObject)?.BindingContext is TodoGetDto tappedDto)
        {
            // Check if the tapped item is the same as the currently selected one
            if (_selectedTodo != null && _selectedTodo == tappedDto)
            {
                // It's the same item, so deselect it and clear the tracker.
                tappedDto.IsSelected = false;
                EnableTimer = false;
                _selectedTodo = null;
            }
            else
            {
                // This is a new item.
                // First, deselect the previous one if it exists.
                if (_selectedTodo != null)
                {
                    _selectedTodo.IsSelected = false;
                }

                // Then, select the new item and update the tracker.
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

}