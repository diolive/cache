using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddPurchasesAndCategories : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"Category",
				table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_Category", x => x.Id); });

			migrationBuilder.CreateTable(
				"Purchase",
				table => new
				{
					Id = table.Column<Guid>(nullable: false),
					CategoryId = table.Column<int>(nullable: false),
					Date = table.Column<DateTime>(nullable: false),
					Name = table.Column<string>(maxLength: 300, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Purchase", x => x.Id);
					table.ForeignKey(
						"FK_Purchase_Category_CategoryId",
						x => x.CategoryId,
						"Category",
						"Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				"IX_Purchase_CategoryId",
				"Purchase",
				"CategoryId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"Purchase");

			migrationBuilder.DropTable(
				"Category");
		}
	}
}