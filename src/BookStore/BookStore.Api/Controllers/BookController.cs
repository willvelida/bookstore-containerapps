using BookStore.Common.Exceptions;
using BookStore.Common.Request;
using BookStore.Common.Response;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string bookId)
        {
            try
            {
                var book = await _bookService.RetrieveBook(bookId);

                return new OkObjectResult(book);
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

        [HttpGet(Name = "GetAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var todoItems = await _bookService.GetAllBooks();

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

        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] BookRequestDto bookRequestDto)
        {
            try
            {
                await _bookService.AddBook(bookRequestDto);

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

        [HttpPut("{bookId}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(string bookId, [FromBody] BookRequestDto bookRequestDto)
        {
            try
            {
                await _bookService.UpdateBook(bookId, bookRequestDto);

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

        [HttpDelete("{bookId}",Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string bookId)
        {
            try
            {
                await _bookService.DeleteBook(bookId);

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
