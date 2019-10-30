using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddCategoryLocalization : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"CategoryLocalization",
				table => new
				{
					CategoryId = table.Column<int>(nullable: false),
					Culture = table.Column<string>(maxLength: 10, nullable: false),
					Name = table.Column<string>(maxLength: 50, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CategoryLocalization", x => new { x.CategoryId, x.Culture });
					table.ForeignKey(
						"FK_CategoryLocalization_Category_CategoryId",
						x => x.CategoryId,
						"Category",
						"Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				"IX_CategoryLocalization_CategoryId",
				"CategoryLocalization",
				"CategoryId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"CategoryLocalization");
		}
	}
}