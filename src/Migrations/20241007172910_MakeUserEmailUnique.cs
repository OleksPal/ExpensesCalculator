using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpensesCalculator.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserEmailUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("029729d7-82c1-4037-ad33-12a4b9b31b76"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("206814c5-52a8-467e-ad94-d5e8021cfa12"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("50752475-7bbb-49d8-8c3f-ac4462e18cbe"), null, "User", "USER" },
                    { new Guid("cab5680c-7ae4-4133-a7ac-c1e5c7a38d3e"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("50752475-7bbb-49d8-8c3f-ac4462e18cbe"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("cab5680c-7ae4-4133-a7ac-c1e5c7a38d3e"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("029729d7-82c1-4037-ad33-12a4b9b31b76"), null, "Admin", "ADMIN" },
                    { new Guid("206814c5-52a8-467e-ad94-d5e8021cfa12"), null, "User", "USER" }
                });
        }
    }
}
