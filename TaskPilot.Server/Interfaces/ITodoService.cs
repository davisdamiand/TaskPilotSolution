using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface ITodoService
    {
        Task CreateTodoAsync(TodoCreateDto todoCreateDto);
    }
}
