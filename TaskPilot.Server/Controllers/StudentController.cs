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
        public async Task<IActionResult> Register([FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Validation failed");

            try
            {
                var studentId = await _registrationService.RegisterStudentWithDefaultsAsync(dto);
                return Ok(studentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Registration failed");
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var success = await _studentService.ResetPasswordAsync(forgotPasswordDto);

                if (success)
                {
                    return Ok(new { Message = "Password has been reset successfully." });
                }

                return BadRequest(new { Message = "Invalid email or date of birth provided." });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        [Route("ValidateStudent")]

        public async Task<IActionResult> ValidateStudent([FromBody] StudentValidationDto studentValidationDto)
        {
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
                    Message = "Authenticating student failed"});
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
