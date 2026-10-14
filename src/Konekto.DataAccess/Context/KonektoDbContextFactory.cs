using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Konekto.DataAccess.Context;

public class KonektoDbContextFactory : IDesignTimeDbContextFactory<KonektoDbContext>
{
    public KonektoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KonektoDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=KonektoDb;User Id=sa;Password=Your_strong_password123!;TrustServerCertificate=True;");

        return new KonektoDbContext(optionsBuilder.Options);
    }
}

