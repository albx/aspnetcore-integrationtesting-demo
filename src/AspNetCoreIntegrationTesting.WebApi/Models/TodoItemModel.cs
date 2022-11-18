namespace AspNetCoreIntegrationTesting.WebApi.Models;

public class TodoItemModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}
