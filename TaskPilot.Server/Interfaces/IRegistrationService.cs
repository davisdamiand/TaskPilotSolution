using Shared.DTOs;
using TaskPilot.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TaskPilot.Server.Interfaces
{
    public interface IRegistrationService
    {
        Task<int> RegisterStudentWithDefaultsAsync(StudentCreateDto dto);
    }
 
}
