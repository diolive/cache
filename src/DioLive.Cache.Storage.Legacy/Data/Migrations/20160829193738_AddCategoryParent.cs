using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddCategoryParent : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				"ParentId",
				"Category",
				nullable: true);

			migrationBuilder.CreateIndex(
				"IX_Category_ParentId",
				"Category",
				"ParentId");

			migrationBuilder.AddForeignKey(
				"FK_Category_Category_ParentId",
				"Category",
				"ParentId",
				"Category",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Category_Category_ParentId",
				"Category");

			migrationBuilder.DropIndex(
				"IX_Category_ParentId",
				"Category");

			migrationBuilder.DropColumn(
				"ParentId",
				"Category");
		}
	}
}