using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TieChef.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffIdToDiningTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "DiningTables",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "DiningTables");
        }
    }
}
