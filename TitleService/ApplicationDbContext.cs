using Microsoft.EntityFrameworkCore;
using TitleService.DbModels;

namespace TitleService;
public class ApplicationDbContext : DbContext
{
    public virtual DbSet<AnimeSeries> AnimeSeries { get; set; }
    public virtual DbSet<Season> Seasons { get; set; }
    public virtual DbSet<Episode> Episodes { get; set; }

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