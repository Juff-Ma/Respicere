namespace Respicere.Server.Data;

using Microsoft.EntityFrameworkCore;
using Shared.Models;

public class DataDbContext : DbContext
{
    public DbSet<SecurityImage> Images { get; set; }

    public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}
}
