using TaskPilot.Client.ViewModels;

namespace TaskPilot.Client;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		
	}

}