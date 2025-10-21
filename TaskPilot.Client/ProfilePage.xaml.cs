using Shared.DTOs;
using TaskPilot.Client.ViewModels;

namespace TaskPilot.Client;

public partial class ProfilePage : ContentPage
{
    private ProfileViewModel ViewModel => BindingContext as ProfileViewModel;

    public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var studentId = Preferences.Get("UserID", 0);
        await ViewModel.LoadStatsAsync(new StatsCalculateDto { StudentID = studentId });
    }

}