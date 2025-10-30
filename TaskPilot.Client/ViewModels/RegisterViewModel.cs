using Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TaskPilot.Client;
using TaskPilot.Client.Services;

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly StudentService _studentService;

    private string _name;
    private string _surname;
    private string _email;
    private string _password;
    private DateTime _dob = DateTime.Today;
    private bool _isBusy;

    public RegisterViewModel(StudentService studentService, StatsService statsService)
    {
        _studentService = studentService;
        RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
        BackToLoginCommand = new Command(async () => await ReturnToLogin());
    }

    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
    public string Surname { get => _surname; set { _surname = value; OnPropertyChanged(); } }
    public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
    public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
    public DateTime DOB { get => _dob; set { _dob = value; OnPropertyChanged(); } }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); }
    }

    public ICommand RegisterCommand { get; }
    public ICommand BackToLoginCommand { get; }

    private Task ReturnToLogin()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var loginPage = MauiProgram.Services.GetService<LoginPage>();
            if (loginPage != null)
            {
                App.Current.MainPage = new NavigationPage(loginPage);
            }
        });

        return Task.CompletedTask;
    }

    private async Task RegisterAsync()
    {
        // Prevent multiple registrations at the same time
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Create the student DTO
            var student = new StudentCreateDto
            {
                Name = Name,
                Surname = Surname,
                Email = Email,
                Password = Password,
                DOB = DateOnly.FromDateTime(DOB)
            };

            // Call the service to register the student
            var id = await _studentService.RegisterStudentWithDefaultsAsync(student);

            // Store user info in Preferences
            Preferences.Set("UserID", id.ToString());
            Preferences.Set("StudentName", Name);
            Preferences.Set("StudentSurname", Surname);

            // Navigate to the main application shell
            await Shell.Current.GoToAsync("///LandingPage");
        }
        catch (Exception ex)
        {
            // Show error message to user
            await Application.Current.MainPage.DisplayAlertAsync("Error", "Registration failed. Please try again.", "OK");
            // log ex somewhere
        }
        finally
        {
            // Reset busy state
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}