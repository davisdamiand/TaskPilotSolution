using Shared.DTOs;

using TaskPilot.Server.Interfaces;
using TaskPilot.Server.Data;
using Microsoft.EntityFrameworkCore;
using TaskPilot.Server.Models;
namespace TaskPilot.Server.Services
{
    public class StatsServices : IStatsService
    {
        private readonly TaskPilotContext _context;

        //Constructor to set the Database context 
        public StatsServices(TaskPilotContext context)
        {
            _context = context;
        }


        public async Task<int> CreateStatsAsync(StatsCreateDto statsCreateDto)
        {
            try
            {
                // Check if the student exist that is making the stats request
                var studentExists = await _context.Students.AnyAsync(s => s.Id == statsCreateDto.StudentID);
                // If not, throw an error
                if (!studentExists)
                    throw new InvalidOperationException("No student with the id for the stats was found");

                // Check if stats for the student already exist
                var StatsAlreadyExist = await _context.Stats.AnyAsync(s => s.StudentID == statsCreateDto.StudentID);
                
                // If yes, throw an error
                if (StatsAlreadyExist)
                    throw new InvalidOperationException("Stats for the student has already been created");

                // Create new stats entry for the student
                var newStats = new Stats
                {
                    Streak = 0,
                    TotalCompletedTasks = 0,
                    TotalPomodoroSessions = 0,
                    StudentID = statsCreateDto.StudentID,
                    LastAccessedDay = null
                };
                // Add the new stats to the database
                await _context.Stats.AddAsync(newStats);
                // Save the changes
                await _context.SaveChangesAsync();
                return newStats.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        // Send the calculated stats
        public async Task<StatsSendDto> SendStatsAsync(StatsCalculateDto statsCalculateDto)
        {
            try
            {
                // Get the stats for the student
                var stats = await _context.Stats.FirstOrDefaultAsync(s => s.StudentID == statsCalculateDto.StudentID);

                // If no stats found, throw an error
                if (stats == null)
                    throw new InvalidOperationException("The Stats for the student has not been created yet");

                // Calculate the stats values
                stats = await CalculateStatsValue(statsCalculateDto, stats);

                // Save the changes to the database
                await _context.SaveChangesAsync();
                // Prepare the stats to send
                var statsToSend = new StatsSendDto
                {
                    TotalPomodoroSessions = stats.TotalPomodoroSessions,
                    TotalCompletedTasks = stats.TotalCompletedTasks,
                    TotalInCompletedTasks = stats.TotalInCompletedTasks,
                    Streak = stats.Streak,
                };

                // Send the stats to the client
                return statsToSend;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<Stats> CalculateStatsValue(StatsCalculateDto statsCalculateDto, Stats stats)
        {
            var totalMinutes = await _context.Todos
                .Where(t => t.StudentID == statsCalculateDto.StudentID)
                .SumAsync(t => t.TimeSpentMinutes);


            stats.TotalPomodoroSessions = totalMinutes / 25;


            stats.TotalCompletedTasks = await _context.Todos
             .Where(t => t.IsCompleted && t.StudentID == statsCalculateDto.StudentID)
             .CountAsync();

            stats.TotalInCompletedTasks = await _context.Todos
            .Where(t => !t.IsCompleted && t.StudentID == statsCalculateDto.StudentID)
            .CountAsync();

            // Persisted streak logic using server-stored LastAccessedDay.
            // If server-stored value not present, fall back to client's dto value.
            var today = DateOnly.FromDateTime(DateTime.Now);

            DateOnly? prior = stats.LastAccessedDay;
            if (!prior.HasValue && statsCalculateDto.LastAccessedDay != default)
            {
                prior = statsCalculateDto.LastAccessedDay;
            }

            if (prior.HasValue)
            {
                if (prior.Value == today)
                {
                    // already visited today — keep streak unchanged
                }
                else if (prior.Value == today.AddDays(-1))
                {
                    // consecutive day -> increment
                    stats.Streak += 1;
                }
                else
                {
                    // gap >= 2 days -> reset
                    stats.Streak = 0;
                }
            }
            else
            {
                // no prior info -> if stats.Streak already > 0 keep it
                stats.Streak = stats.Streak;
            }

            // Update stored last accessed day to today
            stats.LastAccessedDay = today;

            return stats;

        }
    }
}
