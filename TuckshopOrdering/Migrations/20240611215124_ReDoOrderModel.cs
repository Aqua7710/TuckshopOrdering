using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuckshopOrdering.Migrations
{
    /// <inheritdoc />
    public partial class ReDoOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_FoodOrder_FoodOrderID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_FoodOrderID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FoodOrderID",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "FoodOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrder_OrderID",
                table: "FoodOrder",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrder_Order_OrderID",
                table: "FoodOrder",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrder_Order_OrderID",
                table: "FoodOrder");

            migrationBuilder.DropIndex(
                name: "IX_FoodOrder_OrderID",
                table: "FoodOrder");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "FoodOrder");

            migrationBuilder.AddColumn<int>(
                name: "FoodOrderID",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_FoodOrderID",
                table: "Order",
                column: "FoodOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_FoodOrder_FoodOrderID",
                table: "Order",
                column: "FoodOrderID",
                principalTable: "FoodOrder",
                principalColumn: "FoodOrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
