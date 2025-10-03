using Microsoft.EntityFrameworkCore;
using TaskPilot.Server.Models;

namespace TaskPilot.Server.Data
{
    public class TaskPilotContext: DbContext
    {
        //The constructor for the TaskPilotContext
        public TaskPilotContext(DbContextOptions<TaskPilotContext> options)
        : base(options)
        {
        }
        //Represents the Todo Table
        public DbSet<Todo> Todos { get; set; }
        //Represents the Student Table
        public DbSet<Student> Students { get; set; }
        //Represents the Stats Table
        public DbSet<Stats> Stats { get; set; }
    }
}
