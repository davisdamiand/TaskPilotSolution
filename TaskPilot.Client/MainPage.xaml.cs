using Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows.Input;

namespace TaskPilot.Client;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    public List<TodoGetDto> allTodos { get; set; } = new();
    public ObservableCollection<TodoGetDto> listOfTodos { get; set; } = new();

    private bool _isShowingAll = false;
    public string ViewAllButtonText => _isShowingAll ? "Show less" : "View all";

    private readonly HttpClient _httpClient = new HttpClient();

	public string storedID = "";
    public ICommand ViewAllProjectsCommand { get; }
    public MainPage()
	{
		InitializeComponent();

		_httpClient = new HttpClient
		{
			BaseAddress = new Uri(Config.BaseUrl)
		};

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

        await GetTodos(int.Parse(storedID));
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

                // Now update the UI Based on the flag state (view all) or (view top 4)
                UpdateDisplayedTodos();

            }

        }
		catch (Exception)
		{

			//Do nothing
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