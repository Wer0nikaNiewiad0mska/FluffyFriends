using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace User.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRolesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_User_Id",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_User_Id",
                table: "Roles");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "Roles");

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "LastLoginAt", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 4, 17, 3, 13, 737, DateTimeKind.Utc).AddTicks(7119), "admin@example.com", true, null, "AQAAAAIAAYagAAAAEAQvx1fEqgGN97nvGfYb5rxFdieR+XgAZ8qtP2VpJIyjIy1mqrAfc/babNL7tmRytg==", "admin" },
                    { 2, DateTime.UtcNow, "Olalola@gmail.com", true, null, "AQAAAAIAAYagAAAAEAugqRp5dscv9hVzhBVTQasNy+lCvmVAu36v6r3YGBpRwzOPRLwBnIbH3vciLZv9xw==", "Ola" },
                    { 3, DateTime.UtcNow, "Wercia@onet.pl", true, null, "AQAAAAIAAYagAAAAEAtZvz5RPVDaWYaEuF4UXcC0R56UmVBIVe6HK8VyEjGKwbjjBZibA0ITAWDQDWwTjg==", "Weronika" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Employee" },
                    { 3, "Client" }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 }, 
                    { 2, 2 }, 
                    { 3, 2 }  
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.AddColumn<int>(
                name: "User_Id",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "User_Id" },
                values: new object[,]
                {
                    { 1, "Administrator", null },
                    { 2, "Employee", null },
                    { 3, "Client", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "LastLoginAt", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2025, 6, 4, 17, 3, 13, 737, DateTimeKind.Utc).AddTicks(7119), "admin@example.com", true, null, "admin123", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_User_Id",
                table: "Roles",
                column: "User_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_User_Id",
                table: "Roles",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
