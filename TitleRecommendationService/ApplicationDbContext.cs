using Microsoft.EntityFrameworkCore;
using TitleRecommendationService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<TitleRating> TitleRatings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}