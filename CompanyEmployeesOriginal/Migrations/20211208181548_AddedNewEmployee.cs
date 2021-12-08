using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployeesOriginal.Migrations
{
    public partial class AddedNewEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Age", "CompanyId", "Name", "Position" },
                values: new object[] { new Guid("222ca3c1-0deb-4afd-ae94-2159a8479811"), 28, new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), "Or Shalmayev", "Administrator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("222ca3c1-0deb-4afd-ae94-2159a8479811"));
        }
    }
}
