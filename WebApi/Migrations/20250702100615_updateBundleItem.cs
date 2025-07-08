using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class updateBundleItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ProductId",
                table: "BundleItems");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryBatchId",
                table: "BundleItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ProductId_InventoryBatchId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ProductId", "InventoryBatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_InventoryBatchId",
                table: "BundleItems",
                column: "InventoryBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleItems_InventoryBatches_InventoryBatchId",
                table: "BundleItems",
                column: "InventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleItems_InventoryBatches_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ProductId_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropColumn(
                name: "InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ProductId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ProductId" },
                unique: true);
        }
    }
}
