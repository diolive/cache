using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddPlans : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"Plan",
				table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					AuthorId = table.Column<string>(nullable: false),
					BudgetId = table.Column<Guid>(nullable: false),
					BuyDate = table.Column<DateTime>(nullable: true),
					BuyerId = table.Column<string>(nullable: true),
					Comments = table.Column<string>(nullable: true),
					Name = table.Column<string>(maxLength: 300, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Plan", x => x.Id);
					table.ForeignKey(
						"FK_Plan_AspNetUsers_AuthorId",
						x => x.AuthorId,
						"AspNetUsers",
						"Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						"FK_Plan_Budget_BudgetId",
						x => x.BudgetId,
						"Budget",
						"Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						"FK_Plan_AspNetUsers_BuyerId",
						x => x.BuyerId,
						"AspNetUsers",
						"Id",
						onDelete: ReferentialAction.SetNull);
				});

			migrationBuilder.CreateIndex(
				"IX_Plan_AuthorId",
				"Plan",
				"AuthorId");

			migrationBuilder.CreateIndex(
				"IX_Plan_BudgetId",
				"Plan",
				"BudgetId");

			migrationBuilder.CreateIndex(
				"IX_Plan_BuyerId",
				"Plan",
				"BuyerId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"Plan");
		}
	}
}