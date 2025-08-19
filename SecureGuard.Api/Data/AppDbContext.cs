using Microsoft.EntityFrameworkCore;
using SecureGuard.Api.Models;

namespace SecureGuard.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Log> Logs => Set<Log>();
    }
}
