using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddPurchaseAmountAndShop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Purchase",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Shop",
                table: "Purchase",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "Shop",
                table: "Purchase");
        }
    }
}
