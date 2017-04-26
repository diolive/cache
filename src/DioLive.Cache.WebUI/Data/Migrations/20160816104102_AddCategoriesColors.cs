using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddCategoriesColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Category",
                nullable: false,
                defaultValueSql: "ABS(CHECKSUM(NEWID()) % 16777216)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Category");
        }
    }
}
