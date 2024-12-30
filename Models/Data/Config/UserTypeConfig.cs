﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CollegeAppWebAPI.Models.Data.Config
{
    public class UserTypeConfig : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.Name).IsRequired().HasMaxLength(255);
            builder.Property(n => n.Description).HasMaxLength(1500);

            builder.HasData(new List<UserType>()
{
    new UserType
    {
        Id = 1,
        Name = "Student",
        Description = "for students",

    },
    new UserType
    {
        Id = 2,
        Name = "Faculty",
        Description = "for faculty",

    },
    new UserType
    {
        Id = 3,
        Name = "Supporting Staff",
        Description = "for supporting staff",

    },
    new UserType
    {
        Id = 4,
        Name = "Parent",
        Description = "for parent",

    }

     });


        }
    }

}
