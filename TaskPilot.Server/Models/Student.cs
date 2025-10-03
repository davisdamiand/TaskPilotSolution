using System.ComponentModel.DataAnnotations;
namespace TaskPilot.Server.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateOnly DOB { get; set; }

        public virtual List<Todo> Todos { get; set; }
        public virtual Stats stats { get; set; }
    }
}
