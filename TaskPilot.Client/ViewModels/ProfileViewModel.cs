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

        private string _studentName;
        private string _studentSurname;

        public string FullName => $"{StudentName} {StudentSurname}";

        public string StudentName
        {
            get => _studentName;
            set
            {
                _studentName = value;
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

        }

        public async Task LoadStatsAsync(StatsCalculateDto dto)
        {
            Stats = await _profileService.GetStudentStatsAsync(dto);
        }

        private async Task LogoutAsync()
        {
            //Clear all local data
            Preferences.Clear();

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
