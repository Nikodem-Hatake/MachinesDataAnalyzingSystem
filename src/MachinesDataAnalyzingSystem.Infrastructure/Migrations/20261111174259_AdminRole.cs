using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachinesDataAnalyzingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("175a5441-c032-409c-816a-314e1a1be1e9"), "789b5c91-3c55-47d9-802e-ceab7cf2dbc1", "admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("175a5441-c032-409c-816a-314e1a1be1e9"));
        }
    }
}
