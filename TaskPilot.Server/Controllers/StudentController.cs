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

                    return BadRequest(new { errors = errors });
                }

                var id = await _studentService.CreateStudentAsync(studentCreateDto);

                if (id <= 0)
                {
                    return BadRequest( new { error = "Student creation failed — ID was not generated." });
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Student creation failed — ID was not generated." });
            }
        }

    }
}
