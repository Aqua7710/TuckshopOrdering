using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class ModelCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomiseID",
                table: "Menu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customise",
                columns: table => new
                {
                    CustomiseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomiseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customise", x => x.CustomiseID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menu_CustomiseID",
                table: "Menu",
                column: "CustomiseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Customise_CustomiseID",
                table: "Menu",
                column: "CustomiseID",
                principalTable: "Customise",
                principalColumn: "CustomiseID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Customise_CustomiseID",
                table: "Menu");

            migrationBuilder.DropTable(
                name: "Customise");

            migrationBuilder.DropIndex(
                name: "IX_Menu_CustomiseID",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "CustomiseID",
                table: "Menu");
        }
    }
}
