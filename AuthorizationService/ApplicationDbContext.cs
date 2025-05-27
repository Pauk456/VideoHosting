using Microsoft.EntityFrameworkCore;
using AuthorizationService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<Users> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>()
            .ToTable("users", schema: "public");

        base.OnModelCreating(modelBuilder);
    }
}