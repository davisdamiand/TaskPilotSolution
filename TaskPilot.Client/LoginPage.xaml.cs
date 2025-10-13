using System.Net.Http.Json;
using Shared.DTOs;

namespace TaskPilot.Client;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;
    public LoginPage()
	{
		InitializeComponent();

        _httpClient = new HttpClient
        {
            //Stored in a common config class
            BaseAddress = new Uri(Config.BaseUrl)
        };
    }

    private async void RunOnClick(object sender, EventArgs e)
	{
        try
        {
            var student = new StudentValidationDto
            {
                Email = "steve@gmail.com",
                Password = "122445456456546",
            };
            var response = await _httpClient.PostAsJsonAsync("api/Student/ValidateStudent", student);

            NameLabel.Text = response.StatusCode.ToString();
        }
        catch (Exception)
        {
            NameLabel.Text = "Server offline";
        }  
    }
}