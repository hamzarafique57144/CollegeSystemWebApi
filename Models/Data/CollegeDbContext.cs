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
        public DbSet<Student> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<UserRoleMapping> UserRoleMappings { get; set; }
        public DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Student Table
            modelBuilder.ApplyConfiguration(new StudentConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfig());
            modelBuilder.ApplyConfiguration(new UserRoleMappingsConfig());
            modelBuilder.ApplyConfiguration(new UserTypeConfig());

        }

    }
}
 