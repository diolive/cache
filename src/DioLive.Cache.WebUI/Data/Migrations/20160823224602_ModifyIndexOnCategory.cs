using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class ModifyIndexOnCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_OwnerId_Name",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Category_BudgetId_Name",
                table: "Category",
                columns: new[] { "BudgetId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_BudgetId_Name",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Category_OwnerId_Name",
                table: "Category",
                columns: new[] { "OwnerId", "Name" },
                unique: true);
        }
    }
}
