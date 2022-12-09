using AspNetCoreIntegrationTesting.WebApi.Data;
using AspNetCoreIntegrationTesting.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AspNetCoreIntegrationTesting.WebApi.Test;

public class TodoApiTest
{
    [Fact]
    public async Task Get_Todos_Should_Response_With_Ok_Status_Code()
    {
        using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo-InMemory-Test"));
                });
            });

        var httpClient = app.CreateClient();

        var response = await httpClient.GetAsync("/api/todos");
        var items = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemModel>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(items!);
    }

    [Fact]
    public async Task Post_Todos_Should_Response_With_Created_Status_Code_And_Should_Return_The_Created_Item()
    {
        using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo-InMemory-Test"));
                });
            });

        var httpClient = app.CreateClient();

        var model = new TodoItemModel { Title = "test creation", IsComplete = false };

        var response = await httpClient.PostAsJsonAsync("/api/todos", model);
        var responseContent = await response.Content.ReadFromJsonAsync<TodoItemModel>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(model.Title, responseContent!.Title);
        Assert.NotEqual(0, responseContent!.Id);
    }

    [Fact]
    public async Task Post_Todos_Should_Response_With_Bad_Request_Status_Code_And_Should_Return_The_Validation_Errors()
    {
        using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo-InMemory-Test"));
                });
            });

        var httpClient = app.CreateClient();

        var model = new TodoItemModel { Title = "", IsComplete = false };

        var response = await httpClient.PostAsJsonAsync("/api/todos", model);
        var responseContent = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(nameof(model.Title), responseContent.Errors.Keys);
        Assert.Equal("title is required", responseContent.Errors[nameof(model.Title)].First());
    }

    [Fact]
    public async Task Delete_Todos_Should_Response_With_Ok_Status_Code_And_Remove_Todo_Item_Correctly()
    {
        int todoItemId = 0;

        using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo-InMemory-Test"));

                    using var scope = services.BuildServiceProvider().CreateScope();
                    using var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

                    var item = new TodoItem { Title = "test", CreationDate = DateTime.UtcNow, IsComplete = false };
                    context.Todos.Add(item);
                    context.SaveChanges();

                    todoItemId = item.Id;
                });
            });

        var httpClient = app.CreateClient();

        var response = await httpClient.DeleteAsync($"/api/todos/{todoItemId}");

        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(context.Todos, t => t.Id == todoItemId);
    }
}
