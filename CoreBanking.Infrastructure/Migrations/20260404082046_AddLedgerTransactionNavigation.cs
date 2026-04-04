using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLedgerTransactionNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReversedTransactionId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReversedTransactionId1",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReversedTransactionId1",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReversedTransactionId1",
                table: "Transactions",
                column: "ReversedTransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId1",
                table: "Transactions",
                column: "ReversedTransactionId1",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
