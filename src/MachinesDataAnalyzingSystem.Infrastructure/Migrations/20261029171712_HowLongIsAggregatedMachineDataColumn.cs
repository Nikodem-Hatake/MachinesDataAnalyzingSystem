using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachinesDataAnalyzingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HowLongIsAggregatedMachineDataColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HowLongIsAggregatedMachineData",
                table: "AggregatedMachineDatas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowLongIsAggregatedMachineData",
                table: "AggregatedMachineDatas");
        }
    }
}
