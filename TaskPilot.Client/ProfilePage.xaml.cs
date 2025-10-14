namespace TaskPilot.Client;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

	private async void OnLogoutButtonClicked(object sender, EventArgs e)
	{
		// Clear user session or authentication tokens here
		Preferences.Clear();
        // Navigate to the login page
        await Shell.Current.GoToAsync("//LoginPage");
    }
}