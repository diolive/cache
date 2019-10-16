using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddUserTokenFK : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Share_BudgetId",
				"Share");

			migrationBuilder.DropIndex(
				"IX_Options_UserId",
				"Options");

			migrationBuilder.DropIndex(
				"IX_CategoryLocalization_CategoryId",
				"CategoryLocalization");

			migrationBuilder.DropIndex(
				"IX_Category_BudgetId",
				"Category");

			migrationBuilder.DropIndex(
				"IX_Category_BudgetId_Name",
				"Category");

			migrationBuilder.DropIndex(
				"UserNameIndex",
				"AspNetUsers");

			migrationBuilder.DropIndex(
				"IX_AspNetUserRoles_UserId",
				"AspNetUserRoles");

			migrationBuilder.DropIndex(
				"RoleNameIndex",
				"AspNetRoles");

			migrationBuilder.CreateIndex(
				"IX_Category_BudgetId_Name",
				"Category",
				new[] { "BudgetId", "Name" },
				unique: true,
				filter: "[BudgetId] IS NOT NULL AND [Name] IS NOT NULL");

			migrationBuilder.CreateIndex(
				"UserNameIndex",
				"AspNetUsers",
				"NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				"RoleNameIndex",
				"AspNetRoles",
				"NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			migrationBuilder.AddForeignKey(
				"FK_AspNetUserTokens_AspNetUsers_UserId",
				"AspNetUserTokens",
				"UserId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_AspNetUserTokens_AspNetUsers_UserId",
				"AspNetUserTokens");

			migrationBuilder.DropIndex(
				"IX_Category_BudgetId_Name",
				"Category");

			migrationBuilder.DropIndex(
				"UserNameIndex",
				"AspNetUsers");

			migrationBuilder.DropIndex(
				"RoleNameIndex",
				"AspNetRoles");

			migrationBuilder.CreateIndex(
				"IX_Share_BudgetId",
				"Share",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Options_UserId",
				"Options",
				"UserId",
				unique: true);

			migrationBuilder.CreateIndex(
				"IX_CategoryLocalization_CategoryId",
				"CategoryLocalization",
				"CategoryId");

			migrationBuilder.CreateIndex(
				"IX_Category_BudgetId",
				"Category",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Category_BudgetId_Name",
				"Category",
				new[] { "BudgetId", "Name" },
				unique: true);

			migrationBuilder.CreateIndex(
				"UserNameIndex",
				"AspNetUsers",
				"NormalizedUserName",
				unique: true);

			migrationBuilder.CreateIndex(
				"IX_AspNetUserRoles_UserId",
				"AspNetUserRoles",
				"UserId");

			migrationBuilder.CreateIndex(
				"RoleNameIndex",
				"AspNetRoles",
				"NormalizedName");
		}
	}
}