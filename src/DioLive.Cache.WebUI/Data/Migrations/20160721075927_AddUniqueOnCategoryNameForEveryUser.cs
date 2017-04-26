using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddUniqueOnCategoryNameForEveryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                maxLength: 300,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Category_OwnerId_Name",
                table: "Category",
                columns: new[] { "OwnerId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_OwnerId_Name",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                nullable: true);
        }
    }
}
