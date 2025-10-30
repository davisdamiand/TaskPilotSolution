using Microsoft.Maui.Controls;
using Shared.DTOs;
using System;
using TaskPilot.Client.Services;

namespace TaskPilot.Client
{
    public partial class ForgotPasswordPage : ContentPage
    {
        
        private readonly StudentService _studentService;
        private string verifiedEmail;

        public ForgotPasswordPage(StudentService studentService)
        {
            InitializeComponent();
            _studentService = studentService;
        }


        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            // Clear previous error messages
            ErrorLabel.IsVisible = false;

            // --- Gather Input ---
            string email = EntryEmail.Text?.Trim();
            string newPass = EntryNewPassword.Text;
            string confirmPass = EntryConfirmPassword.Text;
            DateTime selectedDate = (DateTime)DatePickerDOB.Date;
            DateOnly dob = DateOnly.FromDateTime(selectedDate);

            // --- Input Validation ---
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPass))
            {
                ErrorLabel.Text = "Please fill in all fields.";
                ErrorLabel.IsVisible = true;
                return;
            }

            // Check if the date of birth is valid
            if (newPass != confirmPass)
            {
                ErrorLabel.Text = "Passwords do not match.";
                ErrorLabel.IsVisible = true;
                return;
            }


            // --- API Call ---
            try
            {
                // Create DTO
                var dto = new ForgotPasswordDto
                {
                    Email = email,
                    DOB = dob,
                    NewPassword = newPass
                };

                // Call the service to reset the password
                bool success = await _studentService.ResetPasswordAsync(dto);

                if (success)
                {
                    await DisplayAlertAsync("Success", "Your password has been reset successfully.", "OK");
                    // Navigate on success
                    await LoginPage();
                }
            }
            catch (Exception ex)
            {
                // Display the error from the server
                ErrorLabel.Text = ex.Message;
                ErrorLabel.IsVisible = true;
            }
        }



        // Back to login
        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
           await LoginPage();
        }


        private async Task LoginPage()
        {
            var serviceProvider = this.Handler.MauiContext.Services;
            var loginPage = serviceProvider.GetService<LoginPage>();
            // clear any stored preferences if needed
            Preferences.Clear();
            Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
