using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CollegeAppWebAPI.Models.Data.Config
{
    public class UserRoleMappingsConfig : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.ToTable("UserRoleMappings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.HasIndex(n => new { n.UserId, n.RoleId},"UK_UserRoleMapping").IsUnique();
            builder.Property(n => n.UserId).IsRequired();
            builder.Property(n => n.RoleId);

            builder.HasOne(n => n.Role)
                .WithMany(n => n.UserRoleMappings)
                .HasForeignKey(n => n.RoleId)
                .HasConstraintName("FK_UserRoleMapping_Role");

            builder.HasOne(n => n.User)
                .WithMany(n => n.UserRoleMappings)
                .HasForeignKey(n => n.UserId)
                .HasConstraintName("FK_UserRoleMapping_User");



        }
    }

}
