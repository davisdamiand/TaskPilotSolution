using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskPilot.Client
{
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            string email = EntryResetEmail.Text?.Trim();

            // Validate email
            if (string.IsNullOrEmpty(email))
            {
                EmailErrorLabel.Text = "Please enter your email address.";
                EmailErrorLabel.IsVisible = true;
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                EmailErrorLabel.Text = "Please enter a valid email address.";
                EmailErrorLabel.IsVisible = true;
                return;
            }

            EmailErrorLabel.IsVisible = false;

            // Simulate sending email
            await Task.Delay(1500);

            MessageLabel.Text = "A password reset link has been sent to your email.";
            MessageLabel.IsVisible = true;
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
