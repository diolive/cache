using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddPurchaseLastEditor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase");

            migrationBuilder.AddColumn<string>(
                name: "LastEditorId",
                table: "Purchase",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_LastEditorId",
                table: "Purchase",
                column: "LastEditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_AspNetUsers_LastEditorId",
                table: "Purchase",
                column: "LastEditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_AspNetUsers_LastEditorId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Purchase_LastEditorId",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "LastEditorId",
                table: "Purchase");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_AspNetUsers_AuthorId",
                table: "Purchase",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
