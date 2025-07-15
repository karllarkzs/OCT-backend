using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDiscountable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountable",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscountable",
                table: "Products");
        }
    }
}
