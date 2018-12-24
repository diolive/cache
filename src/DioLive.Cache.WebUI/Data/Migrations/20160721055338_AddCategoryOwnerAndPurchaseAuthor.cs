using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddCategoryOwnerAndPurchaseAuthor : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				"AuthorId",
				"Purchase",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				"OwnerId",
				"Category",
				nullable: true);

			migrationBuilder.Sql("UPDATE dbo.Purchase SET AuthorId=(SELECT TOP 1 Id FROM dbo.AspNetUsers)");

			migrationBuilder.CreateIndex(
				"IX_Purchase_AuthorId",
				"Purchase",
				"AuthorId");

			migrationBuilder.CreateIndex(
				"IX_Category_OwnerId",
				"Category",
				"OwnerId");

			migrationBuilder.AddForeignKey(
				"FK_Category_AspNetUsers_OwnerId",
				"Category",
				"OwnerId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase",
				"AuthorId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Category_AspNetUsers_OwnerId",
				"Category");

			migrationBuilder.DropForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase");

			migrationBuilder.DropIndex(
				"IX_Purchase_AuthorId",
				"Purchase");

			migrationBuilder.DropIndex(
				"IX_Category_OwnerId",
				"Category");

			migrationBuilder.DropColumn(
				"AuthorId",
				"Purchase");

			migrationBuilder.DropColumn(
				"OwnerId",
				"Category");
		}
	}
}