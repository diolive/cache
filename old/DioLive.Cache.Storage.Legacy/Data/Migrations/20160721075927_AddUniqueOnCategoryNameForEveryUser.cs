using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddUniqueOnCategoryNameForEveryUser : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				"Name",
				"Category",
				maxLength: 300,
				nullable: false);

			migrationBuilder.CreateIndex(
				"IX_Category_OwnerId_Name",
				"Category",
				new[] { "OwnerId", "Name" },
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				"IX_Category_OwnerId_Name",
				"Category");

			migrationBuilder.AlterColumn<string>(
				"Name",
				"Category",
				nullable: true);
		}
	}
}