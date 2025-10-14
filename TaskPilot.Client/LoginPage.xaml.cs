using System.Net.Http.Json;
using System.Threading.Tasks;
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
            BaseAddress = new Uri(Config.BaseUrl)
        };

    }

    //Navigate to forget password page
    private void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ForgotPasswordPage");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        //Set the values
        string email = EntryEmail.Text?.Trim();
        string password = EntryPassword.Text?.Trim();

        var loginStudent = new StudentValidationDto
        {
            Email = email,
            Password = password
        };

        ButtonSignUp.IsEnabled = false;

        await ValidateStudent(loginStudent);

        ButtonSignUp.IsEnabled = true;
    }

    // Navigation 
    private void OnSignUpClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//RegisterPage");
    }

    private async Task ValidateStudent(StudentValidationDto studentValidationDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Student/ValidateStudent", studentValidationDto);

            if (response.IsSuccessStatusCode)
            {
                var id = await response.Content.ReadFromJsonAsync<int>();

                Preferences.Set("UserID", id.ToString());

                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                throw new Exception("Failed to login student");
            }
        }
        catch (Exception)
        {

          await  DisplayAlertAsync("Error", "Failed to login student", "Ok");
        }
    }

}