using Microsoft.EntityFrameworkCore;
using WebBackend.Models;

public class JudgeDbContext : DbContext
{
    //magic function to make EF work
    public JudgeDbContext(DbContextOptions<JudgeDbContext> options) : base(options) 
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<ChallengeLanguage> ChallengesLanguages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Language>().HasData(
            new Language { Id = 1, Name = "c" },
            new Language { Id = 2, Name = "python" }
        );
    }

}