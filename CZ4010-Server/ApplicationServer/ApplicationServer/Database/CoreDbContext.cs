using ApplicationServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApplicationServer
{
    public class CoreDbContext: DbContext
    {
        public DbSet<FileDataModel> Files { get; set; }
        public DbSet<SharingDataModel> Sharing { get; set; }
        
        public DbSet<AuditLogModel> Logs { get; set; }
        
        public CoreDbContext(DbContextOptions<CoreDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileDataModel>().HasKey(x => x.URL);
            modelBuilder.Entity<SharingDataModel>().HasKey(x => new { x.URL, x.TaggedUsername });
        }
    }
    
    public class CoreContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            optionsBuilder.UseNpgsql("Postgres");

            return new CoreDbContext(optionsBuilder.Options);
        }
    }
    
}