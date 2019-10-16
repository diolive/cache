using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddBudget : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Purchase_Category_CategoryId",
				"Purchase");

			migrationBuilder.CreateTable(
				"Budget",
				table => new
				{
					Id = table.Column<Guid>(nullable: false),
					AuthorId = table.Column<string>(nullable: true),
					Name = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Budget", x => x.Id);
					table.ForeignKey(
						"FK_Budget_AspNetUsers_AuthorId",
						x => x.AuthorId,
						"AspNetUsers",
						"Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.AddColumn<Guid>(
				"BudgetId",
				"Purchase",
				nullable: false,
				defaultValue: Guid.Empty);

			migrationBuilder.AddColumn<Guid>(
				"BudgetId",
				"Category",
				nullable: true);

			migrationBuilder.Sql("INSERT INTO dbo.Budget (Id, AuthorId, Name) SELECT NEWID(), Id, N'Home' FROM dbo.AspNetUsers; UPDATE dbo.Category SET BudgetId = b.Id FROM dbo.Budget b INNER JOIN dbo.Category c ON b.AuthorId = c.OwnerId; UPDATE dbo.Purchase SET BudgetId = b.Id FROM dbo.Budget b INNER JOIN dbo.Purchase p ON b.AuthorId = p.AuthorId");

			migrationBuilder.CreateIndex(
				"IX_Purchase_BudgetId",
				"Purchase",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Category_BudgetId",
				"Category",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Budget_AuthorId",
				"Budget",
				"AuthorId");

			migrationBuilder.AddForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category",
				"BudgetId",
				"Budget",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Purchase_Budget_BudgetId",
				"Purchase",
				"BudgetId",
				"Budget",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				"FK_Purchase_Category_CategoryId",
				"Purchase",
				"CategoryId",
				"Category",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Category_Budget_BudgetId",
				"Category");

			migrationBuilder.DropForeignKey(
				"FK_Purchase_Budget_BudgetId",
				"Purchase");

			migrationBuilder.DropForeignKey(
				"FK_Purchase_Category_CategoryId",
				"Purchase");

			migrationBuilder.DropIndex(
				"IX_Purchase_BudgetId",
				"Purchase");

			migrationBuilder.DropIndex(
				"IX_Category_BudgetId",
				"Category");

			migrationBuilder.DropColumn(
				"BudgetId",
				"Purchase");

			migrationBuilder.DropColumn(
				"BudgetId",
				"Category");

			migrationBuilder.DropTable(
				"Budget");

			migrationBuilder.AddForeignKey(
				"FK_Purchase_Category_CategoryId",
				"Purchase",
				"CategoryId",
				"Category",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}