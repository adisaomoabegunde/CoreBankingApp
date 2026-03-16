using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserRoleToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users""
          ALTER COLUMN ""Role""
          TYPE integer
          USING CASE
                WHEN ""Role"" = 'Customer' THEN 0
                WHEN ""Role"" = 'Admin' THEN 1
          END;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users""
          ALTER COLUMN ""Role""
          TYPE text
          USING CASE
                WHEN ""Role"" = 0 THEN 'Customer'
                WHEN ""Role"" = 1 THEN 'Admin'
          END;"
            );
        }
    }
}
