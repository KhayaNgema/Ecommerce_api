using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class EcommerceDbContextFactory : IDesignTimeDbContextFactory<Ecommerce_api.Data.Ecommerce_apiDBContext>
{
    public Ecommerce_api.Data.Ecommerce_apiDBContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Build options
        var optionsBuilder = new DbContextOptionsBuilder<Ecommerce_api.Data.Ecommerce_apiDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new Ecommerce_api.Data.Ecommerce_apiDBContext(optionsBuilder.Options);
    }
}
