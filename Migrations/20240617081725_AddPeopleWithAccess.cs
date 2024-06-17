using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesCalculator.Migrations
{
    /// <inheritdoc />
    public partial class AddPeopleWithAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationPath",
                table: "Checks");

            migrationBuilder.AddColumn<string>(
                name: "PeopleWithAccess",
                table: "Days",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeopleWithAccess",
                table: "Days");

            migrationBuilder.AddColumn<string>(
                name: "VerificationPath",
                table: "Checks",
                type: "TEXT",
                nullable: true);
        }
    }
}
