using BookStore.Common;
using BookStore.Common.Exceptions;
using BookStore.Common.Models;
using BookStore.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookStore.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<BookRepository> logger)
        {
            _cosmosClient = cosmosClient;
            _settings = options.Value;
            _container = _cosmosClient.GetContainer(_settings.databasename, _settings.containername);
            _logger = logger;
        }

        /// <summary>
        /// Creates a book entry in Cosmos DB.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async Task CreateBook(Book book)
        {
            try
            {
                ItemRequestOptions requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(book, new PartitionKey(book.Id), requestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteBook(string bookId)
        {
            try
            {
                ItemResponse<Book> itemResponse = await _container.DeleteItemAsync<Book>(bookId, new PartitionKey(bookId));
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"No Book with ID: {bookId} found!, Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                List<Book> books = new List<Book>();

                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");

                FeedIterator<Book> feedIterator = _container.GetItemQueryIterator<Book>(queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<Book> response = await feedIterator.ReadNextAsync();
                    books.AddRange(response);
                }

                return books;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Book> GetBook(string bookId)
        {
            try
            {
                ItemResponse<Book> itemResponse = await _container.ReadItemAsync<Book>(bookId, new PartitionKey(bookId));

                return itemResponse.Resource;
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"No Book with ID: {bookId} found!, Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task UpdateBook(string bookId, Book book)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.ReplaceItemAsync(book, bookId, new PartitionKey(bookId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
