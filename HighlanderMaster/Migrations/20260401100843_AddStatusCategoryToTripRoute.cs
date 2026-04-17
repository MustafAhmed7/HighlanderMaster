using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HighlanderMaster.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusCategoryToTripRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Routes");
        }
    }
}
