using Microsoft.EntityFrameworkCore;
using MyLogin.Models;
using System.Collections.Generic;

namespace MyLogin
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
