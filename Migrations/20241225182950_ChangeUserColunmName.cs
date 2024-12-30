using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeAppWebAPI.Migrations
{
    public partial class ChangeUserColunmName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "UserTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserTypeId",
                table: "Users",
                newName: "UserType");
        }
    }
}
