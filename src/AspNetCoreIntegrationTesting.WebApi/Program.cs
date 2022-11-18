using AspNetCoreIntegrationTesting.WebApi.Data;
using AspNetCoreIntegrationTesting.WebApi.Models;
using AspNetCoreIntegrationTesting.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationTestingDemoDatabase")));

builder.Services.AddScoped<TodoServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todos", GetAllTodosAsync);
app.MapGet("/todos/{id}", GetTodoItemDetailAsync).WithName("TodoItemDetail");
app.MapPost("/todos", CreateNewTodoItemAsync);
app.MapPut("/todos/{id}", UpdateTodoItemAsync);
app.MapDelete("/todos/{id}", DeleteTodoItemAsync);

app.Run();

#region Endpoints
async Task<IResult> GetAllTodosAsync(TodoServices services)
{
    var todos = await services.GetAllTodosAsync();
    return Results.Ok(todos);
}

async Task<IResult> GetTodoItemDetailAsync(TodoServices services, int id)
{
    var todo = await services.GetTodoItemDetailAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todo);
}

async Task<IResult> CreateNewTodoItemAsync(TodoServices services, [FromBody] TodoItemModel model)
{
    var validation = Validate(model);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.Errors!);
    }

    await services.CreateNewTodoItemAsync(model);
    return Results.CreatedAtRoute("TodoItemDetail", new { id = model.Id }, model);
}

async Task<IResult> UpdateTodoItemAsync(TodoServices services, int id, [FromBody] TodoItemModel model)
{
    var validation = Validate(model);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.Errors!);
    }

    try
    {
        await services.UpdateTodoItemAsync(id, model);
        return Results.Ok();
    }
    catch (ArgumentOutOfRangeException)
    {
        return Results.NotFound();
    }
}

async Task<IResult> DeleteTodoItemAsync(TodoServices services, int id)
{
    try
    {
        await services.DeleteTodoItemAsync(id);
        return Results.Ok();
    }
    catch (ArgumentOutOfRangeException)
    {
        return Results.NotFound();
    }
}


ValidationResult Validate(TodoItemModel model)
{
    if (string.IsNullOrWhiteSpace(model.Title))
    {
        return new ValidationResult(
            false,
            new Dictionary<string, string[]>
            {
                [nameof(model.Title)] = new[] { "title is required" }
            });
    }

    return ValidationResult.Success;
}
#endregion