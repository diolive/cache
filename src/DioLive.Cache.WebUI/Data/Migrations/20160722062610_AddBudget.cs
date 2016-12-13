using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Category_CategoryId",
                table: "Purchase");

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "Purchase",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "Category",
                nullable: true);

            migrationBuilder.Sql("INSERT INTO dbo.Budget (Id, AuthorId, Name) SELECT NEWID(), Id, N'Home' FROM dbo.AspNetUsers; UPDATE dbo.Category SET BudgetId = b.Id FROM dbo.Budget b INNER JOIN dbo.Category c ON b.AuthorId = c.OwnerId; UPDATE dbo.Purchase SET BudgetId = b.Id FROM dbo.Budget b INNER JOIN dbo.Purchase p ON b.AuthorId = p.AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_BudgetId",
                table: "Purchase",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_BudgetId",
                table: "Category",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AuthorId",
                table: "Budget",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Budget_BudgetId",
                table: "Purchase",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Category_CategoryId",
                table: "Purchase",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Budget_BudgetId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Category_CategoryId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Purchase_BudgetId",
                table: "Purchase");

            migrationBuilder.DropIndex(
                name: "IX_Category_BudgetId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Category");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Category_CategoryId",
                table: "Purchase",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}