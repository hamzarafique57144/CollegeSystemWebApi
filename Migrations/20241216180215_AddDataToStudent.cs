using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeAppWebAPI.Migrations
{
    public partial class AddDataToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "DOB", "Email", "StudentName" },
                values: new object[] { 1, "Mansehra, Pakistan", new DateTime(2000, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "abc@gmail.com", "Alice" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "DOB", "Email", "StudentName" },
                values: new object[] { 2, "Abbotabad, Pakistan", new DateTime(1998, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "xyz@gmail.com", "Bob" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "DOB", "Email", "StudentName" },
                values: new object[] { 3, "Mansehra, Pakistan", new DateTime(1995, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "mno@gmail.com", "Charlie" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
