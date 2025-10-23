using Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows.Input;
using TaskPilot.Client.Services;
using System.Linq;

namespace TaskPilot.Client;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    // --- Pomodoro Timer Fields ---
    private const int PomodoroDurationSeconds = 25 * 60; // 25 minutes
    private int _remainingSeconds;
    private IDispatcherTimer _timer;
    private bool _isTimerRunning;
    // -----------------------------

    public List<TodoGetDto> allTodos { get; set; } = new();
    public ObservableCollection<TodoGetDto> listOfTodos { get; set; } = new();
    private bool _isShowingAll = false;
    public string ViewAllButtonText => _isShowingAll ? "Show less" : "View all";
    private readonly HttpClient _httpClient = new HttpClient();
    public string storedID = "";

    private readonly TodoService _todoService;

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

    private void OnTimerTick(object sender, EventArgs e)
    {
        _remainingSeconds--;

        if (_remainingSeconds <= 0)
        {
            // Timer finished
            _timer.Stop();
            _isTimerRunning = false;
            _remainingSeconds = 0;

            // Update button text to reflect a new session is ready
            StartStopButton.Text = "Start New Pomodoro";

            // Display an alert for the break
            DisplayAlert("Pomodoro Finished!", "Time for a short break!", "OK");
        }

        UpdateTimerDisplay();
    }

    // Connects to the button in MainPage.xaml
    private void OnStartStopButtonClicked(object sender, EventArgs e)
    {
        if (_isTimerRunning)
        {
            // Stop/Pause the timer
            _timer.Stop();
            _isTimerRunning = false;
            StartStopButton.Text = "Resume Pomodoro";
        }
        else
        {
            // Start or resume the timer
            if (_remainingSeconds <= 0)
            {
                // Reset to 25:00 if it had finished
                _remainingSeconds = PomodoroDurationSeconds;
            }

            _timer.Start();
            _isTimerRunning = true;
            StartStopButton.Text = "Pause Pomodoro";
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

        try
        {
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
                    todo.IsCompleted = true;
                    allTodos.RemoveAll(t => t.Id == todo.Id);
                    UpdateDisplayedTodos();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update task status: {ex.Message}", "OK");
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

}