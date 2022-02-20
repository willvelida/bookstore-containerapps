using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TodoApi.Common;
using TodoApi.Common.Exceptions;
using TodoApi.Common.Models;
using TodoApi.Repository.Interfaces;

namespace TodoApi.Repository
{
    public class TodoRepository : ITodoRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<TodoRepository> _logger;

        public TodoRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<TodoRepository> logger)
        {
            _cosmosClient = cosmosClient;
            _settings = options.Value;
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
            _logger = logger;
        }

        public async Task CreateTodoItem(TodoItem todoItem)
        {
            try
            {
                ItemRequestOptions requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(todoItem, new PartitionKey(todoItem.Id), requestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteTodoItem(string todoItemId)
        {
            try
            {
                ItemResponse<TodoItem> itemResponse = await _container.DeleteItemAsync<TodoItem>(todoItemId, new PartitionKey(todoItemId));
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"No TodoItem with ID: {todoItemId} found!, Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<TodoItem>> GetAllTodoItems()
        {
            try
            {
                List<TodoItem> todoItems = new List<TodoItem>();

                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");

                FeedIterator<TodoItem> feedIterator = _container.GetItemQueryIterator<TodoItem>(queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<TodoItem> response = await feedIterator.ReadNextAsync();
                    todoItems.AddRange(response);
                }

                return todoItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<TodoItem> GetTodoItem(string todoItemId)
        {
            try
            {
                ItemResponse<TodoItem> itemResponse = await _container.ReadItemAsync<TodoItem>(todoItemId, new PartitionKey(todoItemId));

                return itemResponse.Resource;
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"No TodoItem with ID: {todoItemId} found!, Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task UpdateTodoItem(string todoItemId, TodoItem todoItem)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.ReplaceItemAsync(todoItem, todoItemId, new PartitionKey(todoItemId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
