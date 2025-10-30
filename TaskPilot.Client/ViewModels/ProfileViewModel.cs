using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TaskPilot.Client.Services;

namespace TaskPilot.Client.ViewModels
{
    public class ProfileViewModel: BindableObject
    {
        //1. declare the service used
        private readonly ProfileService _profileService;
        private StatsSendDto _stats;

        public ICommand LogoutCommand { get; }
        public ICommand ReturnHomePageCommand { get; }
        public ICommand ReturnTodoPageCommand { get; }
        public ICommand ReturnCalendarPageCommand { get; }


        private string _studentName;
        private string _studentSurname;
        private int _totalCompletedTasks;
        private int _totalIncompletedTasks;
        private int _totalPomodoroSessions;
        private int _streak;

        public string FullName => $"{StudentName} {StudentSurname}";


        public int Streak
        {
            get => _streak;
            set
            {
                _streak = value;
                OnPropertyChanged();
            }
        }

        public string StudentName
        {
            get => _studentName;
            set
            {
                _studentName = value;
                OnPropertyChanged();
            }
        }

        public int TotalPomodoroSessions
        {
            get => _totalPomodoroSessions;
            set
            {
                _totalPomodoroSessions = value;
                OnPropertyChanged();
            }
        }

        public int TotalCompletedTasks
        {
            get => _totalCompletedTasks;
            set
            {
                _totalCompletedTasks = value;
                OnPropertyChanged();
            }
        }

        public int TotalInCompletedTasks
        {
            get => _totalIncompletedTasks;
            set
            {
                _totalIncompletedTasks = value;
                OnPropertyChanged();
            }
        }

        public string StudentSurname
        {
            get => _studentSurname;
            set
            {
                _studentSurname = value;
                OnPropertyChanged();
            }
        }

        public StatsSendDto Stats
        {
            get => _stats;
            set
            {
                _stats = value;
                OnPropertyChanged();
            }
        }

        public ProfileViewModel(ProfileService profileService)
        {
            _profileService = profileService;
            StudentName = Preferences.Get("StudentName", string.Empty);
            StudentSurname = Preferences.Get("StudentSurname", string.Empty);

            LogoutCommand = new Command(async () => await LogoutAsync());
            ReturnHomePageCommand = new Command(async () => await ReturnHomePageAsync());
            ReturnTodoPageCommand = new Command(async () => await ReturnTodoPageAsync());
            ReturnCalendarPageCommand = new Command(async () => await ReturnCalendarPageAsync());

        }

        public async Task LoadStatsAsync(StatsCalculateDto dto)
        {
            // Call the service to get stats via API call
            Stats = await _profileService.GetStudentStatsAsync(dto);

            // Update properties
            TotalCompletedTasks = Stats.TotalCompletedTasks;
            TotalInCompletedTasks = Stats.TotalInCompletedTasks;
            TotalPomodoroSessions = Stats.TotalPomodoroSessions;
            Streak = Stats.Streak;
        }

        private async Task LogoutAsync()
        {
            //Clear all local data
            Preferences.Clear();
            var loginPage = MauiProgram.Services.GetService<LoginPage>();
            await Shell.Current.GoToAsync("///LoginPage");
        }

        private async Task ReturnHomePageAsync()
        {
            await Shell.Current.GoToAsync("///LandingPage");
        }

        private async Task ReturnTodoPageAsync()
        {
            await Shell.Current.GoToAsync("///TodoPage");
        }

        private async Task ReturnCalendarPageAsync()
        {
            await Shell.Current.GoToAsync("///CalendarPage");
        }
    }
}
