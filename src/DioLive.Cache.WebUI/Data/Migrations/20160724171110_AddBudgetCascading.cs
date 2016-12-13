using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    public partial class AddBudgetCascading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Budget_BudgetId",
                table: "Category",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
