using AspNetCoreIntegrationTesting.WebApi.Data;
using AspNetCoreIntegrationTesting.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegrationTesting.WebApi.Services;

public class TodoServices
{
	private readonly TodoContext context;

	public TodoServices(TodoContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public async Task<IEnumerable<TodoItemModel>> GetAllTodosAsync()
	{
		var todos = await context.Todos
			.Select(t => new TodoItemModel
			{
				Id = t.Id,
				IsComplete = t.IsComplete,
				Title = t.Title
			}).ToArrayAsync();

		return todos;
	}

	public async Task<TodoItemModel?> GetTodoItemDetailAsync(int todoItemId)
	{
		var todo = await context.Todos
			.SingleOrDefaultAsync(t => t.Id == todoItemId);

		if (todo is null)
		{
			return null;
		}

		return new TodoItemModel
		{
			Id = todo.Id,
			IsComplete = todo.IsComplete,
			Title = todo.Title
		};
	}

	public async Task CreateNewTodoItemAsync(TodoItemModel model)
	{
		var todoItem = new TodoItem
		{
			CreationDate = DateTime.UtcNow,
			IsComplete = model.IsComplete,
			Title = model.Title
		};

		context.Add(todoItem);
		await context.SaveChangesAsync();

		model.Id = todoItem.Id;
	}

	public async Task UpdateTodoItemAsync(int todoItemId, TodoItemModel model)
	{
		var todo = await context.Todos
			.SingleOrDefaultAsync(t => t.Id == todoItemId);

		if (todo is null)
		{
			throw new ArgumentOutOfRangeException(nameof(todoItemId));
		}

		todo.Title = model.Title;
		todo.IsComplete = model.IsComplete;
		await context.SaveChangesAsync();
	}

	public async Task DeleteTodoItemAsync(int todoItemId)
	{
		var todo = await context.Todos
			.SingleOrDefaultAsync(t => t.Id == todoItemId);

		if (todo is null)
		{
			throw new ArgumentOutOfRangeException(nameof(todoItemId));
		}

		context.Remove(todo);
		await context.SaveChangesAsync();
	}
}
