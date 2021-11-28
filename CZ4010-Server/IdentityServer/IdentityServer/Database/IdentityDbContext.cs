using System.Diagnostics.CodeAnalysis;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer
{
    public class IdentityDbContext :  DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options):base(options)
        {
        }
        public DbSet<IdentityModel> Identities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityModel>().HasKey(x => x.TaggedUsername);
            modelBuilder.Entity<IdentityModel>().OwnsOne(x => x.PublicKey);
        }
    }
    
    public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseNpgsql("Postgres");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}