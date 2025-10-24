using TaskPilot.Client;

public static class AuthService
{
    public static Page GetInitialPage()
    {
        if (!string.IsNullOrEmpty(Preferences.Get("UserID", null)))
        {
            return MauiProgram.Services.GetService<AppShell>();
        }
        else
        {
            // Use the static Services property to get the fully constructed LoginPage.
            // The DI container knows that LoginPage needs an IServiceProvider.
            return MauiProgram.Services.GetService<LoginPage>();
        }
    }
}