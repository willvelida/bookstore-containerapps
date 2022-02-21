using BookStore.Common;
using BookStore.Repository;
using BookStore.Repository.Interfaces;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<Settings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection("Settings").Bind(settings);
    });
builder.Services.AddSingleton(sp =>
{
    IConfiguration configuration = sp.GetService<IConfiguration>();
    CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
    {
        MaxRetryAttemptsOnRateLimitedRequests = 3,
        MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60),
        ApplicationRegion = "Canada Central",
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    };
    return new CosmosClient(configuration["CosmosDBConnectionString"], cosmosClientOptions);
});
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<IBookService, BookService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
