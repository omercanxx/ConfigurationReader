using ConfigurationReader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        private readonly string connectionString;

        public AppDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(this.connectionString))
            {
                optionsBuilder.UseSqlServer(this.connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store the enum as a string
            modelBuilder.Entity<ConfigurationEntity>()
                .Property(f => f.Type)
                .HasConversion<string>();
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        public DbSet<ConfigurationEntity> Configurations { get; set; }
    }
}
