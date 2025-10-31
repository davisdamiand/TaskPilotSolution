using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Security;
using TaskPilot.Server.Interfaces;
using TaskPilot.Server.Services;

namespace TaskPilot.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IRegistrationService _registrationService;


        public StudentController(IStudentService studentService, IRegistrationService registrationService)
        {
            _studentService = studentService;
            _registrationService = registrationService;

        }

        [HttpPost("Register")]
        // Endpoint to register a new student with default settings
        public async Task<IActionResult> Register([FromBody] StudentCreateDto dto)
        {
            // Validate the incoming DTO
            if (!ModelState.IsValid)
                return BadRequest("Validation failed");

            try
            {
                // Register the student and get the new student ID
                var studentId = await _registrationService.RegisterStudentWithDefaultsAsync(dto);
                return Ok(studentId);
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, use a logging framework)
                Console.WriteLine(ex.Message);
                return BadRequest("Registration failed");
            }
        }

        // Endpoint to reset a student's password
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new ErrorResponse
                {
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            try
            {
                var success = await _studentService.ResetPasswordAsync(forgotPasswordDto);

                if (success)
                {
                    return Ok(new { Message = "Password has been reset successfully." });
                }
                // Should not reach here, but just in case
                return BadRequest(new ErrorResponse { Message = "Unknown error." });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
        }

        // Endpoint to validate a student's information
        [HttpPost]
        [Route("ValidateStudent")]

        // Endpoint to validate a student's credentials
        public async Task<IActionResult> ValidateStudent([FromBody] StudentValidationDto studentValidationDto)
        {
            // Validate the incoming DTO
            try
            {
                // Check for any errors
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });

                }
                // Validate the student and get their ID
                var id = await _studentService.ValidateStudentAsync(studentValidationDto);

                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Student could not be found"
                    });
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ErrorResponse
                {
                    Message = "Authenticating student failed"
                });
            }
        }

        [HttpPost]
        [Route("GetStudentById")]
        public async Task<IActionResult> GetStudentById([FromBody] int id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Student not found"
                    });
                }
                return Ok(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ErrorResponse
                {
                    Message = "Failed to retrieve student"
                });
            }
        }
    }
}
