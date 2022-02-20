using TodoApi.Common.Models;

namespace TodoApi.Repository.Interfaces
{
    public interface ITodoRepository
    {
        Task CreateTodoItem(TodoItem todoItem);
        Task DeleteTodoItem(string todoItemId);
        Task<List<TodoItem>> GetAllTodoItems();
        Task<TodoItem> GetTodoItem(string todoItemId);
        Task UpdateTodoItem(string todoItemId, TodoItem todoItem);
    }
}
