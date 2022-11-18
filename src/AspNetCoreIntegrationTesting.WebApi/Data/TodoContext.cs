using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegrationTesting.WebApi.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Todos { get; set; } = default!;
}
