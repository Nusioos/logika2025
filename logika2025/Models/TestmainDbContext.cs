
using Microsoft.EntityFrameworkCore;
using logika2025.Models;
using System.Runtime.Intrinsics.Arm;

namespace logika2025.Models
{
    public class TestmainDbContext : DbContext
    {
        public DbSet<Testmain> Odp { get; set; }
        public DbSet<Pytania> Pytanie { get; set; }
        public DbSet<User> Users { get; set; }
     
        public TestmainDbContext(DbContextOptions<TestmainDbContext> options) : base(options) 
        {

        }
   
    }
}
