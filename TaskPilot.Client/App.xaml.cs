namespace TaskPilot.Client
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = AuthService.GetInitialPage();
        }

    }
}