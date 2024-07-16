using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class removedHomePageDisplayField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "homePageDisplay",
                table: "Menu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "homePageDisplay",
                table: "Menu",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
