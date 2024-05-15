using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class FieldsForIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageName",
                table: "AspNetUsers");
        }
    }
}
