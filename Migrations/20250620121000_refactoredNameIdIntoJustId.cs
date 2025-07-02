using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class refactoredNameIdIntoJustId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Locations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "InventoryBatches",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FormulationId",
                table: "Formulations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Companies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BundleId",
                table: "Bundles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BundleItemId",
                table: "BundleItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Brands",
                newName: "Id");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpiryDate",
                table: "InventoryBatches",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Locations",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InventoryBatches",
                newName: "BatchId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Formulations",
                newName: "FormulationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Companies",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bundles",
                newName: "BundleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BundleItems",
                newName: "BundleItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Brands",
                newName: "BrandId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "InventoryBatches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
