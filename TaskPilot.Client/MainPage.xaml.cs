namespace TaskPilot.Client;

public partial class MainPage : ContentPage
{
	public string storedID = "";
    public MainPage()
	{
		InitializeComponent();
		
    }

	public async void OnLoad(object sender, EventArgs e)
	{
		storedID = Preferences.Get("UserID", null);
		if (string.IsNullOrEmpty(storedID))
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
        LabelStudentName.Text = Preferences.Get("StudentName", "User") + " " + Preferences.Get("StudentSurname", "Surname");
    }

    public void TodoRedirect()
	{
		Shell.Current.GoToAsync("//TodoPage");
	}

	
}