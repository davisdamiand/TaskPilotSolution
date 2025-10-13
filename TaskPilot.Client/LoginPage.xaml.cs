using System.Net.Http.Json;
using Shared.DTOs;

namespace TaskPilot.Client;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    // Add a reference to the NameLabel control
    private Label NameLabel;

    public LoginPage()
	{
        // Ensure the correct namespace is used for InitializeComponent
        // If this is a .NET MAUI or Xamarin.Forms project, this method is auto-generated
        // and should be available in the partial class.
        this.InitializeComponent(); // Add 'this.' to call the method from the partial class

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Config.BaseUrl)
        };

        // Find the NameLabel control from the XAML by its name
        NameLabel = this.FindByName<Label>("NameLabel");
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