using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using TaskPilot.Server.Interfaces;
using Shared.Security;

namespace TaskPilot.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        [Route("CreateStudent")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    var errors = ModelState
                     .SelectMany(x => x.Value.Errors)
                     .Select(x => x.ErrorMessage)
                     .ToList();

                    return BadRequest(new ErrorResponse { Message = "Validation Failed" } );
                }

                var id = await _studentService.CreateStudentAsync(studentCreateDto);

                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Student creation failed — ID was not generated."
                    });
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Student creation failed — ID was not generated.");
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

    }
}
