using System.ComponentModel.DataAnnotations;

namespace TaskPilot.Server.Models
{
    public class Stats
    {
        [Key]
        public int Id { get; set; }
        public TimeOnly TotalPomoTotalPomodoroSessions { get; set; }
        public int Streak { get; set; }
        public int TotalCompletedTasks { get; set; }

        public int StudentID { get; set; }
        public virtual Student Student { get; set; }
    }
}
