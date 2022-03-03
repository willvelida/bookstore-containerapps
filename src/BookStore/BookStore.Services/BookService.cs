using BookStore.Common.Exceptions;
using BookStore.Common.Models;
using BookStore.Common.Request;
using BookStore.Repository.Interfaces;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task AddBook(BookRequestDto bookRequestDto)
        {
            try
            {
                var book = new Book();
                book.Id = Guid.NewGuid().ToString();
                book.BookName = bookRequestDto.BookName;
                book.Price = bookRequestDto.Price;
                book.Category = bookRequestDto.Category;
                book.Author = bookRequestDto.Author;

                await _bookRepository.CreateBook(book);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in{nameof(AddBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteBook(string bookId)
        {
            try
            {
                await _bookRepository.DeleteBook(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in{nameof(DeleteBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBooks();

            return books;
        }

        public async Task<Book> RetrieveBook(string bookId)
        {
            var book = await _bookRepository.GetBook(bookId);

            if (book is null)
                throw new NotFoundException($"No TodoItem with ID {bookId} exists!");

            return book;
        }

        public async Task UpdateBook(string bookId, BookRequestDto bookRequestDto)
        {
            try
            {
                var bookToUpdate = await _bookRepository.GetBook(bookId);

                if (bookToUpdate is null)
                    throw new NotFoundException($"TodoItem with ID {bookId} not found!");

                bookToUpdate.Author = bookRequestDto.Author;
                bookToUpdate.Price = bookRequestDto.Price;
                bookToUpdate.Category = bookRequestDto.Category;
                bookToUpdate.BookName = bookRequestDto.BookName;

                await _bookRepository.UpdateBook(bookId, bookToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateBook)}: {ex.Message}");
                throw;
            }
        }
    }
}
