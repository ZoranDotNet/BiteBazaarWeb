using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteBazaarWeb.Migrations
{
    /// <inheritdoc />
    public partial class Changedrelationorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FkOrderId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FkApplicationUserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FkOrderId",
                table: "Products",
                column: "FkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_FkOrderId",
                table: "Products",
                column: "FkOrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_FkOrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_FkOrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FkOrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FkApplicationUserId",
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
    }
}
