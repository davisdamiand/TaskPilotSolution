using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using TaskPilot.Server.Interfaces;

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

                    return BadRequest(new { errors = errors } );
                }

                var id = await _studentService.CreateStudentAsync(studentCreateDto);

                if (id <= 0)
                {
                    return BadRequest("Student creation failed — ID was not generated.");
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
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                    return BadRequest(new { errors = errors });
                }

                var id = await _studentService.ValidateStudentAsync(studentValidationDto);

                if (id <= 0)
                {
                    return BadRequest("student needs to register" );
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest($"Authenticating student failed");
            }
        }

    }
}
