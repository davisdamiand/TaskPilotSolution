using System.ComponentModel.DataAnnotations;

namespace TaskPilot.Server.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int PriorityLevel { get; set; }
        public TimeOnly TimeSpent { get; set; }
        public bool IsCompleted { get; set; }

        public int StudentID { get; set; }
        public virtual Student Student { get; set; }
    }
}
