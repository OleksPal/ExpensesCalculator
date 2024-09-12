using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesCalculator.Migrations
{
    /// <inheritdoc />
    public partial class SetCascadeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Users",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Participants",
                table: "Days",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PeopleWithAccess",
                table: "Days",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Users",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Participants",
                table: "Days");

            migrationBuilder.DropColumn(
                name: "PeopleWithAccess",
                table: "Days");
        }
    }
}
