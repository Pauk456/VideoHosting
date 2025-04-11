using Microsoft.EntityFrameworkCore;
using SearchService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<SearchTable> SearchTable { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
}