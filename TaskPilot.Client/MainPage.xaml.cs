using Shared.DTOs;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace TaskPilot.Client;

public partial class MainPage : ContentPage
{
    public List<TodoGetDto> allTodos { get; set; } = new();
    public ObservableCollection<TodoGetDto> listOfTodos { get; set; } = new();

    private readonly HttpClient _httpClient = new HttpClient();

	public string storedID = "";
    public MainPage()
	{
		InitializeComponent();

		_httpClient = new HttpClient
		{
			BaseAddress = new Uri(Config.BaseUrl)
		};

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        storedID = Preferences.Get("UserID", null);

        if (string.IsNullOrEmpty(storedID))
        {
            await Task.Yield();
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        LabelStudentName.Text = Preferences.Get("StudentName", "Name")
                              + " "
                              + Preferences.Get("StudentSurname", "Surname");

        allTodos.Clear();

        await GetTodos(int.Parse(storedID));
        UpdateDashboardTodos();
    }

    public void TodoRedirect()
	{
		Shell.Current.GoToAsync("//TodoPage");
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

                // Now update the UI with only the top 4
                UpdateDashboardTodos();

            }

        }
		catch (Exception)
		{

			throw;
		}
	}

    private void UpdateDashboardTodos()
    {
        listOfTodos.Clear();

        foreach (var todo in allTodos
                            .OrderByDescending(t => t.PrioritySelection) // highest priority first
                            .Take(4))                                   // only top 4
        {
            listOfTodos.Add(todo);
        }
    }


}