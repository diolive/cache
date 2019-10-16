using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddUserOptions : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				"Options",
				table => new
				{
					UserId = table.Column<string>(nullable: false),
					PurchaseGrouping = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Options", x => x.UserId);
					table.ForeignKey(
						"FK_Options_AspNetUsers_UserId",
						x => x.UserId,
						"AspNetUsers",
						"Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				"IX_Options_UserId",
				"Options",
				"UserId",
				unique: true);

			migrationBuilder.Sql("INSERT INTO dbo.[Options] (UserId, PurchaseGrouping) SELECT Id, 2 FROM dbo.AspNetUsers");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				"Options");
		}
	}
}