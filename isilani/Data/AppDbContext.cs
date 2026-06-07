using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using isilani.Models;

namespace isilani.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<IsKategori> IsKategoriler { get; set; }
        public DbSet<IsIlani> IsIlanlari { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IsIlani>().Property(p => p.Maas).HasPrecision(18,2);
        }
    }
}
