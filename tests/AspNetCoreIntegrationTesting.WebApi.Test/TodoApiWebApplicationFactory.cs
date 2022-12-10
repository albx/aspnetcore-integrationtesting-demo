using AspNetCoreIntegrationTesting.WebApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegrationTesting.WebApi.Test;

public class TodoApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo-InMemory-Test"));
        });
    }
}
