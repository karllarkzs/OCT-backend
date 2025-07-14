using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaBack.Migrations
{
    /// <inheritdoc />
    public partial class receiptId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Transactions",
                newName: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptId",
                table: "Transactions",
                newName: "TransactionId");
        }
    }
}
