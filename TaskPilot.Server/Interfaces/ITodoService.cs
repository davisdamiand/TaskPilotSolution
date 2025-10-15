using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface ITodoService
    {
        Task<int> CreateTodoAsync(TodoCreateDto todoCreateDto);

        Task<List<TodoGetDto>> GetTodosByStudentIdAsync(int studentId);
    }
}
