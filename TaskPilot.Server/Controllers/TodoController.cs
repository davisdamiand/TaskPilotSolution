using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using TaskPilot.Server.Interfaces;
using Shared.Security;


namespace TaskPilot.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]

    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost]
        [Route("CreateTodo")]
        public async Task<IActionResult> CreateTodo([FromBody] TodoCreateDto todoCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value.Errors)
                        .Select(x =>  x.ErrorMessage)
                        .ToList();
                    Console.WriteLine("Validation errors: " + string.Join(", ", errors));
                    return BadRequest(errors);
                }

                var id = await _todoService.CreateTodoAsync(todoCreateDto);

                if(id < 0)
                {
                    Console.WriteLine("Todo creation failed - student not found or invalid data");
                    return BadRequest("Todo creation failed - student not found or invalid data");

                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("Todo creation failed -Id was not generated");
            }
        }

        [HttpPost]
        [Route("GetTodos")]
        public async Task<IActionResult> GetTodos([FromBody] int studentId)
        {
            try
            {
                //If something went wrong
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                       .SelectMany(x => x.Value.Errors)
                       .Select(x => x.ErrorMessage)
                       .ToList();

                    Console.WriteLine("Validation errors: " + string.Join(", ", errors));

                    return BadRequest(new ErrorResponse
                    {
                        Message = "Failed to get Todos"
                    });
                }

                var listOfTodos = await _todoService.GetTodosByStudentIdAsync(studentId);

                return Ok(listOfTodos);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return BadRequest(new ErrorResponse
                {
                    Message = "Failed to get todos"
                });
            }
        }
    }
}
