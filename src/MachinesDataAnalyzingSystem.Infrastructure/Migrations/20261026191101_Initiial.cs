using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MachinesDataAnalyzingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initiial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatedMachineDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AverageMotorsSpeed = table.Column<float>(type: "real", nullable: false),
                    AverageTemperature = table.Column<float>(type: "real", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxTemperature = table.Column<float>(type: "real", nullable: false),
                    MinTemperature = table.Column<float>(type: "real", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalNumberOfProcessedResources = table.Column<int>(type: "integer", nullable: false),
                    TotalNumberOfSecondsToProcessResources = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedMachineDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AverageMotorsSpeed = table.Column<float>(type: "real", nullable: false),
                    AverageTemperature = table.Column<float>(type: "real", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumberOfProcessedResources = table.Column<int>(type: "integer", nullable: false),
                    NumberOfSecondsToProcessResources = table.Column<float>(type: "real", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("444efaa8-0acc-4406-9ac2-97ca8e2adfa6"), "Plastic converter" },
                    { new Guid("7556e854-a525-4889-92fd-7412d9d42949"), "Parts assembler" },
                    { new Guid("b656b81f-0d00-47c2-a836-0b0113223cf3"), "Parts cooler" },
                    { new Guid("cec67fd6-9b5a-4b8a-80b3-56dacb845c8c"), "Heat regulator" },
                    { new Guid("e0123916-03a2-4b2d-9598-be733730fb4e"), "Laser cutter" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedMachineDatas");

            migrationBuilder.DropTable(
                name: "MachineDatas");

            migrationBuilder.DropTable(
                name: "Machines");
        }
    }
}
