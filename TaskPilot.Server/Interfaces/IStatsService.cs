using Shared.DTOs;
namespace TaskPilot.Server.Interfaces
{
    public interface IStatsService
    {
        Task<int> CreateStatsAsync(StatsCreateDto statsCreateDto);

        Task<StatsSendDto> SendStatsAsync(StatsCalculateDto statsCalculateDto);

        Task<List<StudentLeague>> SendLeagueStudentsAsync(int currentStudentID);

    }
}
