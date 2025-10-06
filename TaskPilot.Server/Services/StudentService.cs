using TaskPilot.Server.Data;
using Microsoft.EntityFrameworkCore;
using TaskPilot.Server.Models;
using TaskPilot.Server.Interfaces;
using Shared.DTOs;
namespace TaskPilot.Server.Services
{
    public class StudentService: IStudentService
    {
        private readonly TaskPilotContext _context;

        public StudentService(TaskPilotContext context)
        {
            _context = context;
        }

        public async Task<int> CreateStudentAsync(StudentCreateDto studentCreateDto)
        {
            try
            {
                // Format the email to ensure consistency
                var formattedEmail = FormatEmail(studentCreateDto.Email);

                // Ceck if the email is already in use
                var exists = await _context.Students
                .AnyAsync(s => s.Email == formattedEmail);

                if (exists)
                    throw new InvalidOperationException("A student with this email already exists.");

                //Create a new Student entity from the DTO
                var student = new Student
                {
                    Name = studentCreateDto.Name,
                    Surname = studentCreateDto.Surname,
                    Email = formattedEmail,
                    Password = PasswordHelper.HashPassword(studentCreateDto.Password),
                    DOB = studentCreateDto.DOB
                };

                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
                return student.Id;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed

                throw;
            }
            
        }

        public async Task<int> ValidateStudentAsync(StudentValidationDto studentValidationDto)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == FormatEmail(studentValidationDto.Email));
            if (student == null)
            {
                return -1; // Student not found
            }

            if (PasswordHelper.VerifyPassword(student.Password, studentValidationDto.Password))
            {
                return student.Id;
            }

            return -1;
            
        }

        private static string FormatEmail(string email) => email.Trim().ToLowerInvariant();

    }
}
