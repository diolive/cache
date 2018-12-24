using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddBudgetCascading : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category");

			migrationBuilder.AddForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category",
				"BudgetId",
				"Budget",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category");

			migrationBuilder.AddForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category",
				"BudgetId",
				"Budget",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}