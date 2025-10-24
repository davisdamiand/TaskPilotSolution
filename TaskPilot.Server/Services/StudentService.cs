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
                Console.WriteLine(ex.Message);
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

        public async Task<bool> ResetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var formattedEmail = FormatEmail(forgotPasswordDto.Email);

            // Find the student matching BOTH email and DOB for security
            var student = await _context.Students.FirstOrDefaultAsync(s =>
                s.Email == formattedEmail &&
                s.DOB == forgotPasswordDto.DOB);

            // If no student is found, the details were incorrect
            if (student == null)
            {
                return false;
            }

            // If found, update the password with the new hashed password
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
