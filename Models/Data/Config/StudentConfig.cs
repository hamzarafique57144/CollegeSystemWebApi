using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeAppWebAPI.Models.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.StudentName).IsRequired();
            builder.Property(n => n.StudentName).HasMaxLength(255);
            builder.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
            builder.Property(n => n.Email).IsRequired();
            builder.Property(n => n.Email).HasMaxLength(255);
            builder.HasData(new List<Student>()
            {
                new Student
                {
                    Id = 1,
                    StudentName = "Alice",
                    Email = "abc@gmail.com",
                    Address = "Mansehra, Pakistan",
                    DOB = new DateTime(2000, 5, 15) // Example date
                } ,
                new Student
                {
                    Id = 2,
                    StudentName = "Bob",
                    Email = "xyz@gmail.com",
                    Address = "Abbotabad, Pakistan",
                    DOB = new DateTime(1998, 8, 20) // Example date
                },
                 new Student
                 {
                     Id = 3,
                     StudentName = "Charlie",
                     Email = "mno@gmail.com",
                     Address = "Mansehra, Pakistan",
                     DOB = new DateTime(1995, 3, 10) // Example date
                 }
            });
        }
    }
}
