using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class ModifyIndexOnCategory : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Category_OwnerId_Name",
				"Category");

			migrationBuilder.CreateIndex(
				"IX_Category_BudgetId_Name",
				"Category",
				new[] { "BudgetId", "Name" },
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Category_BudgetId_Name",
				"Category");

			migrationBuilder.CreateIndex(
				"IX_Category_OwnerId_Name",
				"Category",
				new[] { "OwnerId", "Name" },
				unique: true);
		}
	}
}