using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddBudgetSharing : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"Share",
				table => new
				{
					BudgetId = table.Column<Guid>(nullable: false),
					UserId = table.Column<string>(nullable: false),
					Access = table.Column<byte>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Share", x => new { x.BudgetId, x.UserId });
					table.ForeignKey(
						"FK_Share_Budget_BudgetId",
						x => x.BudgetId,
						"Budget",
						"Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						"FK_Share_AspNetUsers_UserId",
						x => x.UserId,
						"AspNetUsers",
						"Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				"IX_Share_BudgetId",
				"Share",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Share_UserId",
				"Share",
				"UserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"Share");
		}
	}
}