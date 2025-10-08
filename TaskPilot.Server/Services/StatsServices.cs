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

                if (!studentExists)
                    throw new InvalidOperationException("No student with the id for the stats was found");

                var StatsAlreadyExist = await _context.Stats.AnyAsync(s => s.StudentID == statsCreateDto.StudentID);

                if (StatsAlreadyExist)
                    throw new InvalidOperationException("Stats for the student has already been created");

                var newStats = new Stats
                {
                    Streak = 0,
                    TotalCompletedTasks = 0,
                    TotalPomodoroSessions = 0

                };

                await _context.Stats.AddAsync(newStats);
                await _context.SaveChangesAsync();
                return newStats.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<StatsSendDto> SendStatsAsync(StatsCalculateDto statsCalculateDto)
        {
            try
            {
                var stats = await _context.Stats.FirstOrDefaultAsync(s => s.StudentID == statsCalculateDto.StudentID);

                if (stats == null)
                    throw new InvalidOperationException("The Stats for the student has not been created yet");

                stats = await CalculateStatsValue(statsCalculateDto, stats);

                await _context.SaveChangesAsync();

                var statsToSend = new StatsSendDto
                {
                    TotalPomodoroSessions = stats.TotalPomodoroSessions,
                    TotalCompletedTasks = stats.TotalCompletedTasks,
                    Streak = stats.Streak,
                };

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
                .SumAsync(t => t.TimeSpent.Hour * 60 + t.TimeSpent.Minute);

            stats.TotalPomodoroSessions = totalMinutes / 25;


            stats.TotalCompletedTasks = await _context.Todos
                .Where(t => t.IsCompleted)
                .CountAsync();


            if (statsCalculateDto.LastAccessedDay == DateOnly.FromDateTime(DateTime.Now))
                stats.Streak += 1;
            else
                stats.Streak = 0;

            return stats;

        }
    }
}
