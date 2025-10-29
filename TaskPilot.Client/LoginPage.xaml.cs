using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Security;
using Microsoft.Maui.Storage;

namespace TaskPilot.Client;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(IServiceProvider serviceProvider, HttpClient httpClient)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _httpClient = httpClient;
    }

    //Navigate to forget password page
    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        var serviceProvider = this.Handler.MauiContext.Services;
        var forgotPasswordPage = serviceProvider.GetService<ForgotPasswordPage>();
        Application.Current.MainPage = forgotPasswordPage;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        //Set the values
        string email = EntryEmail.Text?.Trim();
        string password = EntryPassword.Text?.Trim();


        // Create DTO
        var loginStudent = new StudentValidationDto
        {
            Email = email,
            Password = password
        };

        // Disable the button to prevent multiple clicks while processing
        ButtonSignUp.IsEnabled = false;

        await ValidateStudent(loginStudent);

        ButtonSignUp.IsEnabled = true;
    }

    // Navigation 
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        var registerPage = _serviceProvider.GetService<RegisterPage>();
        Application.Current.MainPage = registerPage;
    }

    private async Task ValidateStudent(StudentValidationDto studentValidationDto)
    {
        try
        {
            //Get response from the api
            var response = await _httpClient.PostAsJsonAsync("api/Student/ValidateStudent", studentValidationDto);

            //Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the student ID from the response
                var id = await response.Content.ReadFromJsonAsync<int>();

                //Check for invalid ID
                if (id < 0)
                {
                    ShowError("Invalid email or password");
                    return;
                }

                // Store the student ID in preferences
                await GetStudentInformation(id);
                Preferences.Set("UserID", id.ToString());

                // Navigate to the main application shell
                await Shell.Current.GoToAsync("///LandingPage");
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                {
                    // Show field-specific errors
                    if (errorResponse.Errors.ContainsKey("Email"))
                        EmailErrorLabel.Text = string.Join("\n", errorResponse.Errors["Email"]);

                    if (errorResponse.Errors.ContainsKey("Password"))
                        PasswordErrorLabel.Text = string.Join("\n", errorResponse.Errors["Password"]);

                    EmailErrorLabel.IsVisible = errorResponse.Errors.ContainsKey("Email");
                    PasswordErrorLabel.IsVisible = errorResponse.Errors.ContainsKey("Password");
                }
                else
                {
                    // Fallback to general message
                    ShowError(errorResponse?.Message ?? "An unknown error occurred");
                }
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void ShowError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorLabel.IsVisible = true;
    }

    private async Task GetStudentInformation(int id)
    {
        try
        {
            //Get response from the api
            var response = await _httpClient.PostAsJsonAsync("api/Student/GetStudentById", id);

            //Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the student information from the response
                var student = await response.Content.ReadFromJsonAsync<StudentGetDto>();
                if (student != null)
                {
                    // Store the student information in preferences
                    Preferences.Set("StudentName", student.Name);
                    Preferences.Set("StudentSurname", student.Surname);
                }
                else
                {
                    ShowError("Failed to retrieve student information.");
                }
            }
            else
            {
                ShowError("Failed to retrieve student information.");
            }
        }
        catch (Exception)
        {

            ButtonLogin.BackgroundColor = Colors.Red;
        }
    }


}