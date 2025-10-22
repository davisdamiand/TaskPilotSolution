
namespace TaskPilot.Client
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage, TodoPage todoPage, ProfilePage profilePage, CalendarPage calendarPage, SettingsPage settingsPage)
        {
            InitializeComponent();


            // 1. Create the "Main" Flyout Item
            var mainFlyout = new FlyoutItem { Title = "Main" };
            mainFlyout.Items.Add(new ShellContent
            {
                Title = "Main",
                Content = mainPage, 
                Route = "MainPage"  
            });
            this.Items.Add(mainFlyout);


            // 2. Create the "Todo" Flyout Item
            var todoFlyout = new FlyoutItem { Title = "Todo" };
            todoFlyout.Items.Add(new ShellContent
            {
                Title = "Todo",
                Content = todoPage, // Use the injected instance of TodoPage
                Route = "TodoPage"
            });
            this.Items.Add(todoFlyout);


            // 3. Create the "Profile" Flyout Item
            var profileFlyout = new FlyoutItem { Title = "Profile" };
            profileFlyout.Items.Add(new ShellContent
            {
                Title = "Profile",
                Content = profilePage, // Use the injected instance
                Route = "ProfilePage"
            });
            this.Items.Add(profileFlyout);


            // 4. Create the "Calendar" Flyout Item
            var calendarFlyout = new FlyoutItem { Title = "Calendar" };
            calendarFlyout.Items.Add(new ShellContent
            {
                Title = "Calendar",
                Content = calendarPage, // Use the injected instance
                Route = "CalendarPage"
            });
            this.Items.Add(calendarFlyout);


           // 5.Create the "Settings" Flyout Item
             var settingsFlyout = new FlyoutItem { Title = "Settings" };
            settingsFlyout.Items.Add(new ShellContent
            {
                Title = "Settings",
                Content = settingsPage,
                Route = "SettingsPage"
            });
            this.Items.Add(settingsFlyout);

            // Register routes for pages that should not appear in the flyout
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("StatsPage", typeof(StatsPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(TodoPage), typeof(TodoPage));


        }
    }
}
