using Microsoft.EntityFrameworkCore;
using TitleRecommendationService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<TitleRating> TitleRatings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TitleRating>()
            .ToTable("titlerating", schema: "public"); // явно указываем схему

        base.OnModelCreating(modelBuilder);
    }
}