using TodoApi.Common.Models;
using TodoApi.Common.Request;

namespace TodoApi.Services.Interfaces
{
    public interface ITodoService
    {
        Task AddTodoItem(TodoItemRequestDto todoItemRequestDto);
        Task<TodoItem> RetrieveTodoItem(string todoItemId);
        Task DeleteTodoItem(string todoItemId);
        Task UpdateTodoItem(string todoItemId, TodoItemRequestDto todoItemRequestDto);
        Task<List<TodoItem>> GetAllTodoItems();
    }
}
