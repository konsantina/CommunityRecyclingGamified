using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommunityRecyclingGamified.Data
{
    // Αυτή η κλάση χρησιμοποιείται ΜΟΝΟ στο design-time
    // (δηλαδή όταν τρέχεις Add-Migration / dotnet ef)
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Βάλε εδώ ένα connection string που δουλεύει στον υπολογιστή σου
            var connectionString =
                "Server=(LocalDb)\\ MSSQLLocalDB;Database=CommunityRecyclingDb;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
