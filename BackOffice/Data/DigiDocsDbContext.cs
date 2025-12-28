using BackOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Data
{
    public class DigiDocsDbContext : DbContext
    {

        public DigiDocsDbContext(DbContextOptions<DigiDocsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
            modelBuilder.Entity<Document>()
                .Property(d => d.ContentType)
                .HasConversion<string>();
        }

    }
}
