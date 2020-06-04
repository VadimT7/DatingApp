using DatingApp.API.Controllers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Models.Data

{
    public class DataContext : DbContext //extends DbContext
    {
      public DataContext(DbContextOptions<DataContext> options) : base (options) {}

          public DbSet<Value> Values { get; set; }
      
          public DbSet<User> Users { get; set; }
    }
}