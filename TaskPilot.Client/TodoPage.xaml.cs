
using Shared.DTOs;
using System.Net.Http.Json;

namespace TaskPilot.Client;

public partial class TodoPage : ContentPage
{
	private readonly HttpClient _httpClient = new();



    public TodoPage()
	{
		InitializeComponent();

        DatePickerDueDate.Date = DateTime.Now; // Set default date to today

        _httpClient = new HttpClient
		{
			BaseAddress = new Uri(Config.BaseUrl)
		};
	}

	public async void OnSaveClicked(object sender, EventArgs args)
	{
		// code here
	}
	private async Task CreateTodoAsync(TodoCreateDto todoCreateDto)
	{
		try
		{
			var response = await _httpClient.PostAsJsonAsync("api/Todo/CreateTodo", todoCreateDto);

			if (response.IsSuccessStatusCode)
			{
				var id = await response.Content.ReadFromJsonAsync<int>();
				await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
				throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
		catch (Exception ex)
		{
			await DisplayAlertAsync("Error", ex.Message, "OK");
        }
	}
}