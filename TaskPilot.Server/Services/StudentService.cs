using TaskPilot.Server.Data;
using Microsoft.EntityFrameworkCore;
using TaskPilot.Server.Models;
using TaskPilot.Server.Interfaces;
using Shared.DTOs;
using Shared.Security;

namespace TaskPilot.Server.Services
{
    public class StudentService : IStudentService
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

                // Add student to the database
                await _context.Students.AddAsync(student);

                // Save changes
                await _context.SaveChangesAsync();
                return student.Id;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        // Validate student credentials and return their ID if valid
        public async Task<int> ValidateStudentAsync(StudentValidationDto studentValidationDto)
        {
            // Find the student by formatted email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == FormatEmail(studentValidationDto.Email));
            if (student == null)
            {
                return -1; // Student not found
            }

            // Verify the password
            if (PasswordHelper.VerifyPassword(student.Password, studentValidationDto.Password))
            {
                return student.Id;
            }

            return -1;

        }

        public async Task<bool> ResetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var formattedEmail = FormatEmail(forgotPasswordDto.Email);

            var student = await _context.Students.FirstOrDefaultAsync(s =>
                s.Email == formattedEmail &&
                s.DOB == forgotPasswordDto.DOB);

            if (student == null)
            {
                // Check if email exists at all
                bool emailExists = await _context.Students.AnyAsync(s => s.Email == formattedEmail);
                // Instead of throwing, return false or use a result object
                throw new Exception(emailExists
                    ? "Date of birth does not match our records."
                    : "No account found with this email address.");
            }

            student.Password = PasswordHelper.HashPassword(forgotPasswordDto.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        private static string FormatEmail(string email) => email.Trim().ToLowerInvariant();

        public async Task<StudentGetDto> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return null; // Or throw an exception if preferred
            }
            return new StudentGetDto
            {
                Name = student.Name,
                Surname = student.Surname
            };
        }
    }

}
