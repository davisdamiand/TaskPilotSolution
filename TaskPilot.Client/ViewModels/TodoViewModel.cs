using Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TaskPilot.Client.Services;

public class TodoViewModel : INotifyPropertyChanged
{

    // 1. Fields backing your properties
    
    private readonly TodoService _todoService;
    public TodoViewModel(TodoService todoService)
    {
        _todoService = todoService;
        _dueDateTime = DateTime.Now.AddDays(1).Date.AddHours(17);
        SaveCommand = new Command(async () => await SaveAsync());

    }

    private string _name;
    private string _description;
    private DateTime _dueDateTime;
    private int _priority = 5;

    // 2. Properties bound to UI controls
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    public DateTime DueDateTime
    {
        get => _dueDateTime;
        set { _dueDateTime = value; OnPropertyChanged(); }
    }

    public int Priority
    {
        get => _priority;
        set { _priority = value; OnPropertyChanged(); }
    }

    // 3. Commands bound to buttons
    public ICommand SaveCommand { get; }

    // 4. Logic (calls your ASP.NET API)
    private async Task SaveAsync()
    {
        var storedID = Preferences.Get("UserID", null);
        if (!int.TryParse(storedID, out var studentID))
            throw new InvalidOperationException("Invalid UserID");

        var newTodo = new TodoCreateDto
        {
            StudentId = studentID,
            Name = Name?.Trim(),
            Description = Description?.Trim(),
            DueDateTime = DueDateTime,
            PriorityLevel = Priority

        };

        // Call your API here with HttpClient
        try
        {
            var id = await _todoService.CreateTodoAsync(newTodo);
        }
        catch (Exception ex)
        {

            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // 5. Boilerplate for property change notifications
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}