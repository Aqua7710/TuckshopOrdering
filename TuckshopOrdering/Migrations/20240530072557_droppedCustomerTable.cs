using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class droppedCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrder_Customer_CustomerID",
                table: "FoodOrder");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_FoodOrder_CustomerID",
                table: "FoodOrder");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "FoodOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "FoodOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    roomNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrder_CustomerID",
                table: "FoodOrder",
                column: "CustomerID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrder_Customer_CustomerID",
                table: "FoodOrder",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
