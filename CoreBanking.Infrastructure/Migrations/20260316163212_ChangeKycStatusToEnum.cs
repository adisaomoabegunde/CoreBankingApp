using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKycStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE ""Customers"" SET ""KYCStatus"" = 'Pending' WHERE ""KYCStatus"" IS NULL OR ""KYCStatus"" NOT IN ('Pending', 'Verified', 'Rejected');"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE ""Customers""
          ALTER COLUMN ""KYCStatus""
          TYPE integer
          USING CASE
                WHEN ""KYCStatus"" = 'Pending' THEN 0
                WHEN ""KYCStatus"" = 'Verified' THEN 1
                WHEN ""KYCStatus"" = 'Rejected' THEN 2
                ELSE 0
          END;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Customers"" 
          ALTER COLUMN ""KYCStatus"" 
          TYPE text 
          USING CASE 
                WHEN ""KYCStatus"" = 0 THEN 'Pending'
                WHEN ""KYCStatus"" = 1 THEN 'Verified'
                WHEN ""KYCStatus"" = 2 THEN 'Rejected'
          END;"
            );
        }
    }
}
