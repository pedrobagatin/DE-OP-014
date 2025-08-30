using Ada.UrlShortner.Models;
using Microsoft.EntityFrameworkCore;

namespace Ada.UrlShortner.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UrlRecord> UrlRecords { get; set; }
    public DbSet<User> Users { get; set; }
}
