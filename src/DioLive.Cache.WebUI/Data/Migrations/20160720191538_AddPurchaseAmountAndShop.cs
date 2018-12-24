using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddPurchaseAmountAndShop : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				"Amount",
				"Purchase",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				"Shop",
				"Purchase",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				"Amount",
				"Purchase");

			migrationBuilder.DropColumn(
				"Shop",
				"Purchase");
		}
	}
}