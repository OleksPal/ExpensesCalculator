using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesCalculator.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Index for Items.CheckId - used in joins and WHERE clauses
            migrationBuilder.CreateIndex(
                name: "IX_Items_CheckId",
                table: "Items",
                column: "CheckId");

            // Index for Checks.DayExpensesId - used in joins
            migrationBuilder.CreateIndex(
                name: "IX_Checks_DayExpensesId",
                table: "Checks",
                column: "DayExpensesId");

            // Unique index for Users.UserName - used in lookups
            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_CheckId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Checks_DayExpensesId",
                table: "Checks");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
