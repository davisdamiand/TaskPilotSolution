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

        [HttpPost]
        [Route("UpdateTodo")]
        public async Task<IActionResult> UpdateTodo([FromBody] TodoUpdateDto todoUpdateDto)
        {
            var success = await _todoService.UpdateTodoAsync(todoUpdateDto);

            if (!success)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Todo not found",
                    Errors = new Dictionary<string, string[]>
            {
                { "Id", new[] { "The specified Todo item could not be found." } }
            }
                });
            }

            return Ok(new { Message = "Todo updated successfully" });
        }


        [HttpPost]
        [Route("ToggleCompletion")]
        public async Task<IActionResult> ToggleCompletion([FromBody] int id)
        {
            var success = await _todoService.ToggleTodoCompletionAsync(id);

            if (!success)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Todo not found",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Id", new[] { "The specified Todo item could not be found." } }
                    }
                });
            }

            return Ok(new { Message = "Todo updated successfully" });
        }

        [HttpPost]
        [HttpDelete("TodoDelete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _todoService.DeleteTodoAsync(id);

                if (!success)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Todo not found",
                        Errors = new Dictionary<string, string[]>
                {
                    { "Id", new[] { $"No Todo item with id {id} exists." } }
                }
                    });
                }

                return Ok(new { Message = "Todo deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception internally (ILogger, Application Insights, etc.)
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Message = "An unexpected error occurred while deleting the Todo.",
                    Errors = new Dictionary<string, string[]>
            {
                { "Server", new[] { "Please try again later or contact support if the issue persists." } }
            }
                });
            }
        }


        [HttpPost]
        [HttpPatch("UpdateTime")]
        public async Task<IActionResult> UpdateTimeSpent(int id, [FromQuery] int minutes)
        {
            try
            {
                var success = await _todoService.UpdateTimeSpentAsync(id, minutes);

                if (!success)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Todo not found",
                        Errors = new Dictionary<string, string[]>
                {
                    { "Id", new[] { $"No Todo item with id {id} exists." } }
                }
                    });
                }

                return Ok(new { Message = "Time spent updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception internally (ILogger, Application Insights, etc.)
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Message = "An unexpected error occurred while updating time spent.",
                    Errors = new Dictionary<string, string[]>
            {
                { "Server", new[] { "Please try again later or contact support if the issue persists." } }
            }
                });
            }
        }

    }
}
