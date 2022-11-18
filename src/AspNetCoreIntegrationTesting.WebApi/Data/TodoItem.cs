namespace AspNetCoreIntegrationTesting.WebApi.Data;

public class TodoItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreationDate { get; set; }

    public bool IsComplete { get; set; }
}
