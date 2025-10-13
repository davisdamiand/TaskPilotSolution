namespace TaskPilot.Client;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public void TodoRedirect()
	{
		Shell.Current.GoToAsync("//TodoPage");
	}
}