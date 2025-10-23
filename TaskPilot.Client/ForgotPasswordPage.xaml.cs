using Microsoft.Maui.Controls;
using System;

namespace TaskPilot.Client
{
    public partial class ForgotPasswordPage : ContentPage
    {
        // Stores verified user details temporarily
        private string verifiedEmail;
        private string verifiedDOB;

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        // Verify email and DOB
        private void OnVerifyClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;
            MessageLabel.IsVisible = false;

            string email = EntryEmail.Text?.Trim();
            string dob = EntryDOB.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(dob))
            {
                ErrorLabel.Text = "Please enter both email and date of birth.";
                ErrorLabel.IsVisible = true;
                return;
            }

            // Store values; replace with actual database/API check later
            verifiedEmail = email;
            verifiedDOB = dob;

            // Show new password fields
            NewPasswordSection.IsVisible = true;
        }

        // Reset password
        private void OnResetPasswordClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;
            MessageLabel.IsVisible = false;

            string newPass = EntryNewPassword.Text;
            string confirmPass = EntryConfirmPassword.Text;

            if (string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                ErrorLabel.Text = "Please enter and confirm your new password.";
                ErrorLabel.IsVisible = true;
                return;
            }

            if (newPass != confirmPass)
            {
                ErrorLabel.Text = "Passwords do not match.";
                ErrorLabel.IsVisible = true;
                return;
            }

            // TODO: Update your database/API here
            // Example: UpdateUserPassword(verifiedEmail, verifiedDOB, newPass);

            MessageLabel.Text = "Password reset successfully!";
            MessageLabel.IsVisible = true;

            // Hide password section and clear fields
            NewPasswordSection.IsVisible = false;
            EntryEmail.Text = "";
            EntryDOB.Text = "";
            EntryNewPassword.Text = "";
            EntryConfirmPassword.Text = "";
        }

        // Back to login
        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
