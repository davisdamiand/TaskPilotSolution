namespace TaskPilot.Client;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        Preferences.Remove("UserID");

    }

	public async void OnLoad(object sender, EventArgs e)
	{
		var storedID = Preferences.Get("UserID", null);
		if (string.IsNullOrEmpty(storedID))
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
    }

    public void TodoRedirect()
	{
		Shell.Current.GoToAsync("//TodoPage");
	}
}