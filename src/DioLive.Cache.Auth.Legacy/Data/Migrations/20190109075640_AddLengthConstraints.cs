using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
	public partial class AddLengthConstraints : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				"Shop",
				"Purchase",
				maxLength: 200,
				nullable: true,
				oldClrType: typeof(string),
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Comments",
				"Purchase",
				maxLength: 500,
				nullable: true,
				oldClrType: typeof(string),
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Comments",
				"Plan",
				maxLength: 500,
				nullable: true,
				oldClrType: typeof(string),
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Name",
				"Budget",
				maxLength: 200,
				nullable: false,
				oldClrType: typeof(string),
				oldNullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				"Shop",
				"Purchase",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 200,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Comments",
				"Purchase",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 500,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Comments",
				"Plan",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 500,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				"Name",
				"Budget",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 200);
		}
	}
}