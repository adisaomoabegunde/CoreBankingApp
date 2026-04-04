using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionSelfReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReversedTransactionId1",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReversedTransactionId",
                table: "Transactions",
                column: "ReversedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReversedTransactionId1",
                table: "Transactions",
                column: "ReversedTransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId",
                table: "Transactions",
                column: "ReversedTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId1",
                table: "Transactions",
                column: "ReversedTransactionId1",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_ReversedTransactionId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReversedTransactionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReversedTransactionId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReversedTransactionId1",
                table: "Transactions");
        }
    }
}
