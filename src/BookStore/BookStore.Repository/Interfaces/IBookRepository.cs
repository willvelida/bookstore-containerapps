using BookStore.Common.Models;

namespace BookStore.Repository.Interfaces
{
    public interface IBookRepository
    {
        Task CreateBook(Book book);
        Task DeleteBook(string bookId);
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBook(string bookId);
        Task UpdateBook(string bookId, Book book);
    }
}
