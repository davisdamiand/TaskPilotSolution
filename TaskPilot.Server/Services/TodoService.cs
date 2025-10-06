using TaskPilot.Server.Interfaces;
using Shared.DTOs;

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

        public async Task CreateTodoAsync(TodoCreateDto todoCreateDto)
        {
            try
            {
                // check if student exist to assign the task to that student
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == todoCreateDto.StudentId);

                if (student == null)
                    throw new Exception($"There is no student with the {todoCreateDto.StudentId} in the database");



            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public int PriorityEngine(int currentLevel, DateOnly DueDate)
        {
            // Required code
            return 0;


       
        }
    }
}
