using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesKitTask.Migrations
{
    /// <inheritdoc />
    public partial class updateEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItemVM_AspNetUsers_AppUserId",
                table: "BasketItemVM");

            migrationBuilder.DropIndex(
                name: "IX_BasketItemVM_AppUserId",
                table: "BasketItemVM");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "BasketItemVM");

            migrationBuilder.RenameColumn(
                name: "LinkeednLink",
                table: "Employees",
                newName: "LinkedinLink");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "LinkedinLink",
                table: "Employees",
                newName: "LinkeednLink");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "BasketItemVM",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemVM_AppUserId",
                table: "BasketItemVM",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItemVM_AspNetUsers_AppUserId",
                table: "BasketItemVM",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
