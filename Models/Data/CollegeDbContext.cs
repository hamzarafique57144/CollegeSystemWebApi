using CollegeAppWebAPI.Models.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace CollegeAppWebAPI.Models.Data
{
    public class CollegeDbContext : DbContext
    {
        public CollegeDbContext(DbContextOptions<CollegeDbContext> options):base(options)  
        {
                
        }
       public DbSet<Student> Students { get; set; } 
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Student Table
            modelBuilder.ApplyConfiguration(new StudentConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());

        }

    }
}
 