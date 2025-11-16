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

        // value used to push completed tasks to the end of ordered lists
        private const double CompletedPriorityValue = 10;

        private const double PriorityTieBreakerWeight = 0.01;

        public TodoService (TaskPilotContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleTodoCompletionAsync(int todoId)
        {
            var todo = await _context.Todos.FindAsync(todoId);
            if (todo == null) return false;

            // Toggle completion state
            todo.IsCompleted = !todo.IsCompleted;

            // If completed, push to the end by assigning a very large priority selection.
            // If un-completed, recalc priority based on existing rules.
            if (todo.IsCompleted)
            {
                todo.PrioritySelection = CompletedPriorityValue;
            }
            else
            {
                todo.PrioritySelection = CalculatePriority(todo);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTodoAsync(int todoId)
        {
            // Check if todo exists
            var todo = await _context.Todos.FindAsync(todoId);
            if (todo == null) return false;

            // Remove todo
            _context.Todos.Remove(todo);
            // Save changes
            await _context.SaveChangesAsync();
            // Return success
            return true;
        }

        public async Task<bool> UpdateTodoAsync(TodoUpdateDto dto)
        {
            // Find the todo item
            var todo = await _context.Todos.FindAsync(dto.Id);
            if (todo == null) return false;

            // Update fields from the DTO to the entity
            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.DueDateTime = dto.DueDateTime;
            todo.PriorityLevel = dto.PriorityLevel;
    
            if (!todo.IsCompleted)
            {
                todo.PrioritySelection = CalculatePriority(todo);
            }

            // Save changes
            await _context.SaveChangesAsync();
            // Return success
            return true;
        }


        public async Task<bool> UpdateTimeSpentAsync(int todoId, int minutes)
        {
            var todo = await _context.Todos.FindAsync(todoId);
            if (todo == null) return false;

            todo.TimeSpentMinutes += minutes;
            await _context.SaveChangesAsync();

            return true;
        }


        //Create todo object
        public async Task<int> CreateTodoAsync(TodoCreateDto todoCreateDto)
        {
            try
            {
                // check if student exist to assign the task to that student
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == todoCreateDto.StudentId);
                Console.WriteLine(student);
                if (student == null)
                    throw new InvalidOperationException($"There is no student with the {todoCreateDto.StudentId} in the database");

                // create new todo object
                var newTodo = new Todo
                {
                    StudentID = student.Id,
                    Title = todoCreateDto.Name,
                    Description = todoCreateDto.Description,
                    DueDateTime = todoCreateDto.DueDateTime,
                    PriorityLevel = todoCreateDto.PriorityLevel,
                };

                // calculate priority selection
                newTodo.PrioritySelection = CalculatePriority(newTodo);

                // add todo to database
                await _context.Todos.AddAsync(newTodo);

                // save changes to database
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
            // Calculate exact days until due (negative means overdue)
            double daysUntilDue = (todo.DueDateTime - DateTime.Now).TotalDays;

            // Use a very small fraction (1e-8) for priority to ensure exact time always dominates
            // Maximum priority difference (4 levels * 1e-8 = 4e-8) is smaller than 1 second (≈1.157e-8 days)
            double priorityAdjustment = todo.PriorityLevel * PriorityTieBreakerWeight;
            // Store the calculated value
            double finalScore = daysUntilDue + priorityAdjustment;

            todo.PrioritySelection = finalScore;

            return finalScore;
        }

        //Get all the todos belonging to a specific user (completed tasks are returned too,
        //but completed tasks will have a large PrioritySelection so they appear last)
        public async Task<List<TodoGetDto>> GetTodosByStudentIdAsync(int studentID)
        {
            // Query todos for the specified student, ordered by PrioritySelection
            var listOfTodos = await _context.Todos
            .Where(s => s.StudentID == studentID)
            .OrderBy(t => t.PrioritySelection)
            .Select(t => new TodoGetDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                PrioritySelection = t.PrioritySelection,
                PriorityLevel = t.PriorityLevel,
                TimeSpentMinutes = t.TimeSpentMinutes,
                DueDateTime = t.DueDateTime,
                StartDateTime = t.StartDateTime,
                EndDateTime = t.EndDateTime,
                IsCompleted = t.IsCompleted
            })
            .ToListAsync();

            return listOfTodos;
        }
    }
}
