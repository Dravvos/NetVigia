using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class ServerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdTGHTTPMethod",
                table: "Servers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IdTGHTTPMethod",
                table: "Servers",
                column: "IdTGHTTPMethod");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_TabelaGeralItem_IdTGHTTPMethod",
                table: "Servers",
                column: "IdTGHTTPMethod",
                principalTable: "TabelaGeralItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_TabelaGeralItem_IdTGHTTPMethod",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_IdTGHTTPMethod",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "IdTGHTTPMethod",
                table: "Servers");
        }
    }
}
