using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeAppWebAPI.Migrations
{
    public partial class CreatingRolePrivilegeRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_RoleID",
                table: "RolePrivileges",
                column: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Roles",
                table: "RolePrivileges",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Roles",
                table: "RolePrivileges");

            migrationBuilder.DropIndex(
                name: "IX_RolePrivileges_RoleID",
                table: "RolePrivileges");
        }
    }
}
