namespace TaskPilot.Client
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void OnRegisterClicked(object sender, EventArgs e)
        {
            var name = EntryName.Text;
        }
    }
}
