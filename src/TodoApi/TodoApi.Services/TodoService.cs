using Microsoft.Extensions.Logging;
using TodoApi.Common.Exceptions;
using TodoApi.Common.Models;
using TodoApi.Common.Request;
using TodoApi.Repository.Interfaces;
using TodoApi.Services.Interfaces;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task AddTodoItem(TodoItemRequestDto todoItemRequestDto)
        {
            try
            {
                var todoItem = new TodoItem();
                todoItem.Id = Guid.NewGuid().ToString();
                todoItem.Title = todoItemRequestDto.Title;
                todoItem.IsComplete = todoItemRequestDto.IsComplete;

                await _todoRepository.CreateTodoItem(todoItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in{nameof(AddTodoItem)}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteTodoItem(string todoItemId)
        {
            try
            {
                await _todoRepository.DeleteTodoItem(todoItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in{nameof(DeleteTodoItem)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TodoItem>> GetAllTodoItems()
        {
            var todoItems = new List<TodoItem>();

            todoItems = await _todoRepository.GetAllTodoItems();

            return todoItems;
        }

        public async Task<TodoItem> RetrieveTodoItem(string todoItemId)
        {
            var todoItem = await _todoRepository.GetTodoItem(todoItemId);

            if (todoItem is null)
                throw new NotFoundException($"No TodoItem with ID {todoItemId} exists!");

            return todoItem;
        }

        public async Task UpdateTodoItem(string todoItemId, TodoItemRequestDto todoItemRequestDto)
        {
            try
            {
                var todoToUpdate = await _todoRepository.GetTodoItem(todoItemId);

                if (todoToUpdate is null)
                    throw new NotFoundException($"TodoItem with ID {todoItemId} not found!");

                todoToUpdate.Title = todoItemRequestDto.Title;
                todoToUpdate.IsComplete = todoItemRequestDto.IsComplete; ;

                await _todoRepository.UpdateTodoItem(todoItemId, todoToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateTodoItem)}: {ex.Message}");
                throw;
            }
        }
    }
}
