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

    // Constructor
    public TodoViewModel(TodoService todoService)
    {
        _todoService = todoService;
        _dueDateTime = DateTime.Now.AddDays(1).Date.AddHours(17);
        SaveCommand = new Command(async () => await SaveAsync());
        DeleteCommand = new Command(async () => await DeleteAsync());

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

    private async Task DeleteAsync()
    {
        if (!_isEditMode || !_todoId.HasValue)
            return;
        var confirm = await Application.Current.MainPage.DisplayAlertAsync("Confirm Delete", "Are you sure you want to delete this todo?", "Yes", "No");
        if (!confirm)
            return;
        try
        {
            await _todoService.DeleteTodoAsync(_todoId.Value);
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
        var storedID = Preferences.Get("UserID", null);
        if (!int.TryParse(storedID, out var studentID))
            throw new InvalidOperationException("Invalid UserID");

        try
        {
            if (_isEditMode && _todoId.HasValue)
            {
                var updateDto = new TodoUpdateDto
                {
                    Id = _todoId.Value,
                    StudentId = studentID,
                    Title = Name?.Trim(),
                    Description = Description?.Trim(),
                    DueDateTime = DueDateTime,
                    PriorityLevel = int.TryParse(Priority, out var p) ? p : 5
                };
                await _todoService.UpdateTodo(updateDto);
            }
            else
            {
                var newTodo = new TodoCreateDto
                {
                    StudentId = studentID,
                    Name = Name?.Trim(),
                    Description = Description?.Trim(),
                    DueDateTime = DueDateTime,
                    PriorityLevel = int.TryParse(Priority, out var p) ? p : 5
                };
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