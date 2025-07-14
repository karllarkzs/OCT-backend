using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class FlattenProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleItems_InventoryBatches_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bundles_Locations_LocationId",
                table: "Bundles");

            migrationBuilder.DropTable(
                name: "ConsumableExtensions");

            migrationBuilder.DropTable(
                name: "InventoryBatches");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Bundles_LocationId",
                table: "Bundles");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ProductId_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropColumn(
                name: "IsConsumable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Bundles");

            migrationBuilder.DropColumn(
                name: "InventoryBatchId",
                table: "BundleItems");

            migrationBuilder.DropColumn(
                name: "Uses",
                table: "BundleItems");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Products",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "LowStockThreshold",
                table: "Products",
                newName: "MinStock");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpiryDate",
                table: "Products",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReceivedDate",
                table: "Products",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Bundles",
                type: "text",
                nullable: true);

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
                name: "ExpiryDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Bundles");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "MinStock",
                table: "Products",
                newName: "LowStockThreshold");

            migrationBuilder.AddColumn<bool>(
                name: "IsConsumable",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Bundles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryBatchId",
                table: "BundleItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Uses",
                table: "BundleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConsumableExtensions",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsesLeft = table.Column<int>(type: "integer", nullable: false),
                    UsesMax = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumableExtensions", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_ConsumableExtensions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    QuantityOnHand = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryBatches_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryBatches_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bundles_LocationId",
                table: "Bundles",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ProductId_InventoryBatchId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ProductId", "InventoryBatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_InventoryBatchId",
                table: "BundleItems",
                column: "InventoryBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_LocationId",
                table: "InventoryBatches",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_ProductId",
                table: "InventoryBatches",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleItems_InventoryBatches_InventoryBatchId",
                table: "BundleItems",
                column: "InventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bundles_Locations_LocationId",
                table: "Bundles",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
