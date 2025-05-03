using AIBridges.Models;
using Microsoft.EntityFrameworkCore;

namespace AIBridges.Data
{
    public class AIBridgesDbContext : DbContext
    {
        public AIBridgesDbContext(DbContextOptions<AIBridgesDbContext> options)
            : base(options)
        {
        }

        public DbSet<AIModel> Models { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}