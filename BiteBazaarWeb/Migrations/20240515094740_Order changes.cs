using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteBazaarWeb.Migrations
{
    /// <inheritdoc />
    public partial class Orderchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_FkOrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_FkOrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FkOrderId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FkOrderId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_FkOrderId",
                table: "Products",
                column: "FkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_FkOrderId",
                table: "Products",
                column: "FkOrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }
    }
}
