using Microsoft.AspNetCore.Mvc;
using TodoApi.Common.Exceptions;
using TodoApi.Common.Request;
using TodoApi.Common.Response;
using TodoApi.Services.Interfaces;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpGet("{todoItemId}")]
        public async Task<IActionResult> Get(string todoItemId)
        {
            try
            {
                var todoItem = await _todoService.RetrieveTodoItem(todoItemId);

                return new OkObjectResult(todoItem);
            }
            catch (NotFoundException nex)
            {
                _logger.LogError(nex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(nex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet(Name = "GetAllTodoItems")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var todoItems = await _todoService.GetAllTodoItems();

                return new OkObjectResult(todoItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost(Name = "CreateTodoItem")]
        public async Task<IActionResult> Create([FromBody] TodoItemRequestDto todoItemRequestDto)
        {
            try
            {
                await _todoService.AddTodoItem(todoItemRequestDto);

                return new CustomRequestObjectResult(null, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{todoItemId}", Name = "UpdateTodoItem")]
        public async Task<IActionResult> Put(string todoItemId, [FromBody] TodoItemRequestDto todoItemRequestDto)
        {
            try
            {
                await _todoService.UpdateTodoItem(todoItemId, todoItemRequestDto);

                return new CustomRequestObjectResult(null, StatusCodes.Status204NoContent);
            }
            catch (NotFoundException nex)
            {
                _logger.LogError(nex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(nex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{todoItemId}",Name = "DeleteTodoItem")]
        public async Task<IActionResult> Delete(string todoItemId)
        {
            try
            {
                await _todoService.DeleteTodoItem(todoItemId);

                return new CustomRequestObjectResult(null, StatusCodes.Status204NoContent);
            }
            catch (NotFoundException nex)
            {
                _logger.LogError(nex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(nex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
