using Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TaskPilot.Client.Services;

namespace TaskPilot.Client.ViewModels;

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly StudentService _studentService;

    private string _name;
    private string _surname;
    private string _email;
    private string _password;
    private DateTime _dob = DateTime.Today;

    public RegisterViewModel(StudentService studentService)
    {
        _studentService = studentService;
        RegisterCommand = new Command(async () => await RegisterAsync(), CanRegister);
    }

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); }
    }

    public string Surname
    {
        get => _surname;
        set { _surname = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); ((Command)RegisterCommand).ChangeCanExecute(); }
    }

    public DateTime DOB
    {
        get => _dob;
        set { _dob = value; OnPropertyChanged(); }
    }

    public ICommand RegisterCommand { get; }

    private bool CanRegister()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && !string.IsNullOrWhiteSpace(Surname)
            && !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(Password);
    }

    private async Task RegisterAsync()
    {
        var student = new StudentCreateDto
        {
            Name = Name?.Trim(),
            Surname = Surname?.Trim(),
            Email = Email?.Trim(),
            Password = Password,
            DOB = DateOnly.FromDateTime(DOB)
        };

        try
        {
            var id = await _studentService.CreateStudentAsync(student);

            // Store locally
            Preferences.Set("UserID", id.ToString());
            Preferences.Set("StudentName", Name);
            Preferences.Set("StudentSurname", Surname);

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}