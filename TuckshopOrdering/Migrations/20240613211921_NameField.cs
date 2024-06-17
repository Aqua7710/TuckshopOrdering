using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class NameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "roomNumber",
                table: "FoodOrder");

            migrationBuilder.DropColumn(
                name: "studentName",
                table: "FoodOrder");

            migrationBuilder.AddColumn<string>(
                name: "roomNumber",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "studentName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "roomNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "studentName",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "roomNumber",
                table: "FoodOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "studentName",
                table: "FoodOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
