using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class FKCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Menu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Menu_CategoryID",
                table: "Menu",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Category_CategoryID",
                table: "Menu",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Category_CategoryID",
                table: "Menu");

            migrationBuilder.DropIndex(
                name: "IX_Menu_CategoryID",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Menu");
        }
    }
}
