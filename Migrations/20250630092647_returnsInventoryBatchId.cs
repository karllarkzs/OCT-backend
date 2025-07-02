using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class returnsInventoryBatchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "InventoryBatches");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Bundles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bundles_LocationId",
                table: "Bundles",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bundles_Locations_LocationId",
                table: "Bundles",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bundles_Locations_LocationId",
                table: "Bundles");

            migrationBuilder.DropIndex(
                name: "IX_Bundles_LocationId",
                table: "Bundles");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Bundles");

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "InventoryBatches",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
