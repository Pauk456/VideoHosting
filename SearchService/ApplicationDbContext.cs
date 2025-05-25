using Microsoft.EntityFrameworkCore;
using SearchService.DbModels;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<SearchData> SearchData { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SearchData>()
            .ToTable("searchdata", schema: "public"); // явно указываем схему

        base.OnModelCreating(modelBuilder);
    }
}