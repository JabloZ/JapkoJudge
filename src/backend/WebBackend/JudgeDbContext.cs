using Microsoft.EntityFrameworkCore;
using WebBackend.Models;

public class JudgeDbContext : DbContext
{
    //magic function to make EF work
    public JudgeDbContext(DbContextOptions<JudgeDbContext> options) : base(options) 
    {
    }

    public DbSet<User> Users { get; set; }
}