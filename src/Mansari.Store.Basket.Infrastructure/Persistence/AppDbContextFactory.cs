using Mansari.Store.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mansari.Store.Basket.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-I8S0JTC\\SQL2022;Database=Manstore;User Id=sa;Password=!QAZ1qaz;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
        );
        return new AppDbContext(optionsBuilder.Options);
    }
}

