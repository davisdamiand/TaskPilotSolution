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