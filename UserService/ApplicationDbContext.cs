using Microsoft.EntityFrameworkCore;
using UserService.DbModels;

public class ApplicationDbContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<UserAnime> UserAnime {  get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAnime>()
            .HasKey(ua => new { ua.IdUser, ua.IdAnime }); // Составной ключ

        

        modelBuilder.Entity<Users>().ToTable("users");

        base.OnModelCreating(modelBuilder);
    }
}