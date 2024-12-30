using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeAppWebAPI.Models.Data.Config
{
    public class RolePrivilegeConfig : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
    {
        builder.ToTable("RolePrivileges");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(n => n.RolePrivilegeName).IsRequired().HasMaxLength(250);
        builder.Property(n => n.Description);
        builder.Property(n => n.IsActive).IsRequired();
        builder.Property(n => n.IsDeleted).IsRequired();
        builder.Property(n => n.CreatedDate).IsRequired();

            builder.HasOne(n => n.Role)
                .WithMany(n => n.RolePrivileges)
                .HasForeignKey(n => n.RoleID)
                .HasConstraintName("FK_RolePrivileges_Roles");

        }
}
}
