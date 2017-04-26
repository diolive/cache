using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddPurchaseComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Purchase",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Purchase");
        }
    }
}
