using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string _studentName;
        private string _studentSurname;
        private int _totalCompletedTasks;
        private int _totalIncompletedTasks;
        private int _totalPomodoroSessions;
        private int _streak;

        public ICommand LogoutCommand { get; }
        public ICommand ReturnHomePageCommand { get; }
        public ICommand ReturnTodoPageCommand { get; }
        public ICommand ReturnCalendarPageCommand { get; }

        public ObservableCollection<StudentLeague> LeagueStudents { get; set; }

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

        // --- Avatar Cycling Logic ---
        private readonly string[] _avatars = new[]
        {
            "avatar1.jpeg",
            "avatar2.jpeg",
            "avatar3.jpeg",
            "avatar4.jpeg",
            "avatar5.jpeg"
        };

        private int _avatarIndex;
        public int AvatarIndex
        {
            get => _avatarIndex;
            set
            {
                if (_avatarIndex != value)
                {
                    _avatarIndex = value;
                    Preferences.Set("AvatarIndex", _avatarIndex);
                    OnPropertyChanged(nameof(AvatarImageSource));
                }
            }
        }

        public string AvatarImageSource => _avatars[AvatarIndex];

        public ICommand NextAvatarCommand { get; }
        public ICommand PrevAvatarCommand { get; }

        public void NextAvatar()
        {
            AvatarIndex = (AvatarIndex + 1) % _avatars.Length;
        }

        public void PrevAvatar()
        {
            AvatarIndex = (AvatarIndex - 1 + _avatars.Length) % _avatars.Length;
        }
        // ---------------------------

        public ProfileViewModel(ProfileService profileService)
        {
            _profileService = profileService;
            StudentName = Preferences.Get("StudentName", string.Empty);
            StudentSurname = Preferences.Get("StudentSurname", string.Empty);

            LogoutCommand = new Command(async () => await LogoutAsync());
            ReturnHomePageCommand = new Command(async () => await ReturnHomePageAsync());
            ReturnTodoPageCommand = new Command(async () => await ReturnTodoPageAsync());
            ReturnCalendarPageCommand = new Command(async () => await ReturnCalendarPageAsync());

            // Initialize avatar index from preferences
            _avatarIndex = Preferences.Get("AvatarIndex", 0);

            // Initialize avatar commands
            NextAvatarCommand = new Command(NextAvatar);
            PrevAvatarCommand = new Command(PrevAvatar);
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

        public async Task LoadStudentsLeage(int studentID)
        {
            var leagueStudents = await _profileService.GetLeagueStudentsAsync(studentID);
            LeagueStudents = new ObservableCollection<StudentLeague>(leagueStudents);
            OnPropertyChanged(nameof(LeagueStudents));
        }

        private async Task LogoutAsync()
        {
            //Clear all local data
            Preferences.Clear();
            var loginPage = MauiProgram.Services.GetService<LoginPage>();
            await Shell.Current.GoToAsync("///LoginPage");
        }

        // Navigation functions
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
