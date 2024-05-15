using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteBazaarWeb.Migrations
{
    /// <inheritdoc />
    public partial class OrderUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Carts_FkCartId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_FkCartId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FkCartId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "FkOrderId",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_FkOrderId",
                table: "Carts",
                column: "FkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Orders_FkOrderId",
                table: "Carts",
                column: "FkOrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Orders_FkOrderId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_FkOrderId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "FkOrderId",
                table: "Carts");

            migrationBuilder.AddColumn<int>(
                name: "FkCartId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FkCartId",
                table: "Orders",
                column: "FkCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Carts_FkCartId",
                table: "Orders",
                column: "FkCartId",
                principalTable: "Carts",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
