using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class addedCustomerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "roomNumber",
                table: "FoodOrder");

            migrationBuilder.DropColumn(
                name: "studentName",
                table: "FoodOrder");
        }
    }
}
