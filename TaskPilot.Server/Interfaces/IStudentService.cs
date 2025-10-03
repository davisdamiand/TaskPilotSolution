using TaskPilot.Server.Models;
using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface IStudentService
    {
        Task<int> CreateStudentAsync(StudentCreateDto studentCreateDto);

        Task<bool> ValidateStudentAsync(StudentValidationDto studentValidationDto);
    }
}
