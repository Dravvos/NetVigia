using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntegrationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdTGTypeNotification",
                table: "Integrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_IdTGTypeNotification",
                table: "Integrations",
                column: "IdTGTypeNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_TabelaGeralItem_IdTGTypeNotification",
                table: "Integrations",
                column: "IdTGTypeNotification",
                principalTable: "TabelaGeralItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_TabelaGeralItem_IdTGTypeNotification",
                table: "Integrations");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_IdTGTypeNotification",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "IdTGTypeNotification",
                table: "Integrations");
        }
    }
}
