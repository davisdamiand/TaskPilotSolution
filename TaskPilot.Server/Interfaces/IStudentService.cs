using TaskPilot.Server.Models;
using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface IStudentService
    {
        Task<int> CreateStudentAsync(StudentCreateDto studentCreateDto);

        Task<int> ValidateStudentAsync(StudentValidationDto studentValidationDto);

        Task<StudentGetDto> GetStudentByIdAsync(int id);

        Task<bool> ResetPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    }
}
