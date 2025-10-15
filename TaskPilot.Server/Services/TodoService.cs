using TaskPilot.Server.Interfaces;
using Shared.DTOs;
using TaskPilot.Server.Models;
using TaskPilot.Server.Data;
using Microsoft.EntityFrameworkCore;
namespace TaskPilot.Server.Services
{
    public class TodoService : ITodoService
    {
        private readonly TaskPilotContext _context;
        
        public TodoService (TaskPilotContext context)
        {
            _context = context;
        }

        public async Task<int> CreateTodoAsync(TodoCreateDto todoCreateDto)
        {
            try
            {
                // check if student exist to assign the task to that student
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == todoCreateDto.StudentId);
                Console.WriteLine(student);
                if (student == null)
                    throw new InvalidOperationException($"There is no student with the {todoCreateDto.StudentId} in the database");

                var newTodo = new Todo
                {
                    StudentID = student.Id,
                    Title = todoCreateDto.Name,
                    Description = todoCreateDto.Description,
                    DueDate = todoCreateDto.DueDate,
                    StartTime = todoCreateDto.StartTime,
                    EndTime = todoCreateDto.EndTime,
                    PriorityLevel = todoCreateDto.PriorityLevel,
                };

                newTodo.PrioritySelection = CalculatePriority(newTodo);

                await _context.Todos.AddAsync(newTodo);
                await _context.SaveChangesAsync();

                return newTodo.Id;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return -1;
            }

        }

        public double CalculatePriority(Todo todo)
        {
            // Base score from user
            double score = todo.PriorityLevel;

            // Days until due
            var dueDateTime = todo.DueDate.ToDateTime(todo.EndTime);
            double daysUntilDue = (dueDateTime - DateTime.Now).TotalDays;

            // Duration in hours
            double durationHours = todo.EndTime.ToTimeSpan().TotalHours - todo.StartTime.ToTimeSpan().TotalHours;
            if (durationHours < 0) durationHours += 24; // handle overnight

            // Adjust based on due date
            if (daysUntilDue <= 1) score -= 1.0;
            else if (daysUntilDue <= 3) score -= 0.5;

            // Adjust based on task length
            if (durationHours < 1) score -= 0.5;

            // Store the calculated value
            todo.PrioritySelection = score;

            return score;
        }

        public async Task<List<TodoGetDto>> GetTodosByStudentIdAsync(int studentID)
        {
            var listOfTodos = await _context.Todos
            .Where(s => s.StudentID == studentID)
            .OrderBy(t => t.PrioritySelection)
            .Select(t => new TodoGetDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                PrioritySelection = t.PrioritySelection,
                TimeSpentMinutes = t.TimeSpentMinutes,
                DueDate = t.DueDate,
                StartTime = t.StartTime,
                EndTime = t.EndTime

            })
            .ToListAsync();

            return listOfTodos;
        }
    }
}
