using AutomotiveStock.CentralStock.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CentralStockDbContext>
{
    public CentralStockDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        
        var optionsBuilder = new DbContextOptionsBuilder<CentralStockDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new CentralStockDbContext(optionsBuilder.Options);
    }
}