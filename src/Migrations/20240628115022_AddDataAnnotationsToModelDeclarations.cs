using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesCalculator.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAnnotationsToModelDeclarations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checks_Days_DayExpensesId",
                table: "Checks");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Checks_CheckId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Users",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Participants",
                table: "Days");

            migrationBuilder.DropColumn(
                name: "PeopleWithAccess",
                table: "Days");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Items",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "CheckId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Sum",
                table: "Checks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "DayExpensesId",
                table: "Checks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Checks_Days_DayExpensesId",
                table: "Checks",
                column: "DayExpensesId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Checks_CheckId",
                table: "Items",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checks_Days_DayExpensesId",
                table: "Checks");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Checks_CheckId",
                table: "Items");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Items",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CheckId",
                table: "Items",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

            migrationBuilder.AlterColumn<double>(
                name: "Sum",
                table: "Checks",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "DayExpensesId",
                table: "Checks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Checks_Days_DayExpensesId",
                table: "Checks",
                column: "DayExpensesId",
                principalTable: "Days",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Checks_CheckId",
                table: "Items",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id");
        }
    }
}
