using Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TaskPilot.Client.Services;

public class TodoViewModel : INotifyPropertyChanged, IQueryAttributable
{
    // Fields
    private readonly TodoService _todoService;
    private bool _isEditMode;
    private int? _todoId;
    private string _name;
    private string _description;
    private DateTime _dueDateTime;
    private string _priority = "5"; // Default value

    // UI-Bound Properties
    public string HeaderText => _isEditMode ? "Edit Task" : "Add a New Task";
    public string SaveButtonText => _isEditMode ? "Update Todo" : "Save Todo";

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (_isEditMode != value)
            {
                _isEditMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }
    }
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
        set
        {
            if (_dueDateTime != value)
            {
                _dueDateTime = value;
                OnPropertyChanged();
                // Also notify dependent properties
                OnPropertyChanged(nameof(DueDate));
                OnPropertyChanged(nameof(DueTime));
            }
        }
    }

    public TimeSpan DueTime
    {
        get => _dueDateTime.TimeOfDay;
        set
        {
            if (_dueDateTime.TimeOfDay != value)
            {
                _dueDateTime = _dueDateTime.Date + value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DueDateTime));
            }
        }
    }

    public DateTime DueDate
    {
        get => _dueDateTime.Date;
        set
        {
            if (_dueDateTime.Date != value.Date)
            {
                _dueDateTime = value.Date + _dueDateTime.TimeOfDay;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DueDateTime));
            }
        }
    }

    public string Priority
    {
        get => _priority;
        set
        {
            if (_priority != value)
            {
                _priority = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public ICommand ReturnHomeCommand { get; }
    public ICommand ReturnProfileCommand { get; }
    public ICommand ReturnCalendarCommand { get; }

    // Constructor
    public TodoViewModel(TodoService todoService)
    {
        _todoService = todoService;
        _dueDateTime = DateTime.Now.AddDays(1).Date.AddHours(17);
        SaveCommand = new Command(async () => await SaveAsync());
        DeleteCommand = new Command(async () => await DeleteAsync());
        ReturnHomeCommand = new Command(async () => await ReturnHomeAsync());
        ReturnCalendarCommand = new Command(async () => await ReturnCalendarAsync());
        ReturnProfileCommand = new Command(async () => await ReturnProfileAsync());
        // Set default state
        ResetFields();
    }

    // Method to handle navigation data
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TaskToEdit", out var task) && task is TodoGetDto todoDto)
        {
            LoadTaskForEdit(todoDto);
        }
        else
        {
            ResetFields();
        }
    }

    // Logic to populate fields for editing
    private void LoadTaskForEdit(TodoGetDto task)
    {
        IsEditMode = true;
        _todoId = task.Id;

        // Update properties which will trigger INotifyPropertyChanged
        Name = task.Title;
        Description = task.Description;
        DueDateTime = task.DueDateTime;
        Priority = task.PriorityLevel.ToString(); 

    }

    //Retun back to main page
    private async Task ReturnHomeAsync()
    {
        ResetFields();
        await Shell.Current.GoToAsync("///LandingPage");
    }

    // Navigate to the Calendar Page
    private async Task ReturnCalendarAsync()
    {
        ResetFields();
        await Shell.Current.GoToAsync("///CalendarPage");
    }

    // Navigate to the Profile  Page
    private async Task ReturnProfileAsync()
    {
        ResetFields();
        await Shell.Current.GoToAsync("///ProfilePage");
    }

    private async Task DeleteAsync()
    {
        // Only proceed if in edit mode and ID is available
        if (!_isEditMode || !_todoId.HasValue)
            return;
        // Confirm deletion with the user
        var confirm = await Application.Current.MainPage.DisplayAlertAsync("Confirm Delete", "Are you sure you want to delete this todo?", "Yes", "No");
        if (!confirm)
            return;
        try
        {
            // Call the service to delete the todo via API
            await _todoService.DeleteTodoAsync(_todoId.Value);

            // Reset fields and navigate back to main page
            ResetFields();
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async Task SaveAsync()
    {
        // Validate inputs
        var storedID = Preferences.Get("UserID", null);
        // Ensure UserID is available
        if (!int.TryParse(storedID, out var studentID))
            throw new InvalidOperationException("Invalid UserID");

        try
        {
            // Differentiate between edit and create
            if (_isEditMode && _todoId.HasValue)
            {
                // Prepare the update DTO
                var updateDto = new TodoUpdateDto
                {
                    Id = _todoId.Value,
                    StudentId = studentID,
                    Title = Name?.Trim(),
                    Description = Description?.Trim(),
                    DueDateTime = DueDateTime,
                    PriorityLevel = int.TryParse(Priority, out var p) ? p : 5
                };
                // Call the update method that contain the API call
                await _todoService.UpdateTodo(updateDto);
            }
            else
            {
                //Create new todo
                var newTodo = new TodoCreateDto
                {
                    StudentId = studentID,
                    Name = Name?.Trim(),
                    Description = Description?.Trim(),
                    DueDateTime = DueDateTime,
                    PriorityLevel = int.TryParse(Priority, out var p) ? p : 5
                };
                // Call the create method that contain the API call
                await _todoService.CreateTodoAsync(newTodo);
            }

            ResetFields();
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void ResetFields()
    {
        Name = string.Empty;
        Description = string.Empty;
        DueDateTime = DateTime.Now.AddDays(1).Date.AddHours(17);
        Priority = "5";
        IsEditMode = false;
    }

    // Boilerplate for property change notifications
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}