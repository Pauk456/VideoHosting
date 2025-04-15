using Microsoft.EntityFrameworkCore;
using TitleService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<AnimeSeries> AnimeSeries { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimeSeries>()
            .HasMany(a => a.Seasons)
            .WithOne(s => s.Series)
            .HasForeignKey(s => s.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Season>()
            .HasMany(s => s.Episodes)
            .WithOne(e => e.Season)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}