using System.Threading.Tasks;

namespace TaskPilot.Client;

public partial class MainPage : ContentPage
{
    bool _navigationTriggered;
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Yield();

        if(_navigationTriggered)
            return;

        var userId = Preferences.Get("UserID", string.Empty);
        if (string.IsNullOrEmpty(userId))
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
        else
        {
            await Shell.Current.GoToAsync("///LandingPage");
        }

        _navigationTriggered = true;
    }
}
