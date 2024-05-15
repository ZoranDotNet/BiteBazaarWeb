using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteBazaarWeb.Migrations
{
    /// <inheritdoc />
    public partial class OrderSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderSpecifications",
                columns: table => new
                {
                    OrderSpecificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FkOrderId = table.Column<int>(type: "int", nullable: false),
                    FkProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSpecifications", x => x.OrderSpecificationId);
                    table.ForeignKey(
                        name: "FK_OrderSpecifications_Orders_FkOrderId",
                        column: x => x.FkOrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSpecifications_Products_FkProductId",
                        column: x => x.FkProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderSpecifications_FkOrderId",
                table: "OrderSpecifications",
                column: "FkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSpecifications_FkProductId",
                table: "OrderSpecifications",
                column: "FkProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderSpecifications");
        }
    }
}
