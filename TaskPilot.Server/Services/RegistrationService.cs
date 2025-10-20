using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using TaskPilot.Server.Data;
using TaskPilot.Server.Interfaces;
using TaskPilot.Server.Services;

namespace TaskPilot.Server.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IStudentService _studentService;
        private readonly IStatsService _statsService;
        private readonly TaskPilotContext _dbContext; // or IUnitOfWork abstraction

        public RegistrationService(
            IStudentService studentService,
            IStatsService statsService,
            TaskPilotContext dbContext)
        {
            _studentService = studentService;
            _statsService = statsService;
            _dbContext = dbContext;
        }

        public async Task<int> RegisterStudentWithDefaultsAsync(StudentCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var studentId = await _studentService.CreateStudentAsync(dto);

                var statsDto = new StatsCreateDto { StudentID = studentId };
                await _statsService.CreateStatsAsync(statsDto);

                await transaction.CommitAsync();
                return studentId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
