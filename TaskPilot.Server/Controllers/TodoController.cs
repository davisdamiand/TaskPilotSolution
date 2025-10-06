using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using TaskPilot.Server.Interfaces;


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
                    return BadRequest(errors);
                }

                var id = await _todoService.CreateTodoAsync(todoCreateDto);

                if(id < 0)
                {
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
    }
}
