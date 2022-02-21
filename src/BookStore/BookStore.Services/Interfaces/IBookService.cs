using BookStore.Common.Models;
using BookStore.Common.Request;

namespace BookStore.Services.Interfaces
{
    public interface IBookService
    {
        Task AddBook(BookRequestDto bookRequestDto);
        Task<Book> RetrieveBook(string bookId);
        Task DeleteBook(string bookId);
        Task UpdateBook(string bookId, BookRequestDto bookRequestDto);
        Task<List<Book>> GetAllBooks();
    }
}
