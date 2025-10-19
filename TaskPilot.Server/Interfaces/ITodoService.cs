using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface ITodoService
    {
        Task<int> CreateTodoAsync(TodoCreateDto todoCreateDto);
        Task<List<TodoGetDto>> GetTodosByStudentIdAsync(int studentId);

        Task<bool> UpdateTodoAsync(TodoUpdateDto todoUpdateDto);
        Task<bool> ToggleTodoCompletionAsync(int todoId);
        Task<bool> UpdateTimeSpentAsync(int todoId, int minutes);
        Task<bool> DeleteTodoAsync(int todoId);


    }
}
