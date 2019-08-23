using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddPurchaseLastEditor : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase");

			migrationBuilder.AddColumn<string>(
				"LastEditorId",
				"Purchase",
				nullable: true);

			migrationBuilder.CreateIndex(
				"IX_Purchase_LastEditorId",
				"Purchase",
				"LastEditorId");

			migrationBuilder.AddForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase",
				"AuthorId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				"FK_Purchase_AspNetUsers_LastEditorId",
				"Purchase",
				"LastEditorId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.SetNull);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase");

			migrationBuilder.DropForeignKey(
				"FK_Purchase_AspNetUsers_LastEditorId",
				"Purchase");

			migrationBuilder.DropIndex(
				"IX_Purchase_LastEditorId",
				"Purchase");

			migrationBuilder.DropColumn(
				"LastEditorId",
				"Purchase");

			migrationBuilder.AddForeignKey(
				"FK_Purchase_AspNetUsers_AuthorId",
				"Purchase",
				"AuthorId",
				"AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}