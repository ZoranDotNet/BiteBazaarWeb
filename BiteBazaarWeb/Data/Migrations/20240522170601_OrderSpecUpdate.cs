using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteBazaarWeb.Migrations
{
    /// <inheritdoc />
    public partial class OrderSpecUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PayedPrice",
                table: "OrderSpecifications",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayedPrice",
                table: "OrderSpecifications");
        }
    }
}
