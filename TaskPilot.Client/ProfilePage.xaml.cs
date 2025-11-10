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
        var storedId = Preferences.Get("UserID",Id.ToString());


        //Send today's date to calculate streaks
        var dto = new StatsCalculateDto
        {
            StudentID = int.Parse(storedId),
            LastAccessedDay = DateOnly.FromDateTime(DateTime.Now)
        };

        await ViewModel.LoadStatsAsync(dto);
        await ViewModel.LoadStudentsLeage(int.Parse(storedId));
    }

}