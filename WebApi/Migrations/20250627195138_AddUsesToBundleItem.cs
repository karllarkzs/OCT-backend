using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class AddUsesToBundleItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId",
                table: "BundleItems");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "BundleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Uses",
                table: "BundleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ProductId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ProductId",
                table: "BundleItems");

            migrationBuilder.DropColumn(
                name: "Uses",
                table: "BundleItems");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "BundleItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId",
                table: "BundleItems",
                column: "BundleId");
        }
    }
}
