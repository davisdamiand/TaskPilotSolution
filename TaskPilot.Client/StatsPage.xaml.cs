using Microsoft.Maui.Controls;
using System;

namespace TaskPilot.Client
{
    public partial class StatsPage : ContentPage
    {
        public StatsPage()
        {
            InitializeComponent();
        }

        private async void OnCourseraClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.coursera.org/");
        }

        private async void OnUdemyClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.udemy.com/");
        }

        private async void OnFreeCodeCampClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.freecodecamp.org/");
        }

        private async void OnKhanAcademyClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.khanacademy.org/");
        }

        private async void OnLinkedInLearningClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.linkedin.com/learning/");
        }
    }
}
