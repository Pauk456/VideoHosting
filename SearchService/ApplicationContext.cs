using Microsoft.EntityFrameworkCore;
using SearchService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<SearchData> SearchData { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
}