using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddCategoryOwnerAndPurchaseAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Purchase",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Category",
                nullable: true);

            migrationBuilder.Sql("UPDATE dbo.Purchase SET AuthorId=(SELECT TOP 1 Id FROM dbo.AspNetUsers)");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_AuthorId",
                table: "Purchase",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_OwnerId",
                table: "Category",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_OwnerId",
                table: "Category",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_OwnerId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Purchase_AuthorId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Category_OwnerId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Category");
        }
    }
}