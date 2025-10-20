using Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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

    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var student = new StudentCreateDto
            {
                Name = Name,
                Surname = Surname,
                Email = Email,
                Password = Password,
                DOB = DateOnly.FromDateTime(DOB)
            };

            var id = await _studentService.RegisterStudentWithDefaultsAsync(student);

            Preferences.Set("UserID", id.ToString());
            Preferences.Set("StudentName", Name);
            Preferences.Set("StudentSurname", Surname);

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", "Registration failed. Please try again.", "OK");
            // log ex somewhere
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}