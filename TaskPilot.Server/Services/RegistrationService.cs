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

        // Registers a new student and initializes default settings within a transaction
        public async Task<int> RegisterStudentWithDefaultsAsync(StudentCreateDto dto)
        {
            // Begin a transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create the student
                var studentId = await _studentService.CreateStudentAsync(dto);
                // Initialize default stats for the new student
                var statsDto = new StatsCreateDto { StudentID = studentId };
                
                // This calls the StatsService to create default stats
                await _statsService.CreateStatsAsync(statsDto);

                // Commit the transaction
                await transaction.CommitAsync();
                // Return the new student ID
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
