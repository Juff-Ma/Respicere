using Microsoft.EntityFrameworkCore;
using Respicere.Server.Models;
using System;
using System.Collections.Generic;

namespace Respicere.Server.Data;

public class DataDbContext : DbContext
{
    public DbSet<SecurityImage> Images { get; set; }

    public DataDbContext(DbContextOptions<DataDbContext> options) : base(options){}
}
