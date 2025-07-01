using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntegrationServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Integrations_IntegrationUserModelId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_IntegrationUserModelId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "IntegrationUserModelId",
                table: "Servers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IntegrationUserModelId",
                table: "Servers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IntegrationUserModelId",
                table: "Servers",
                column: "IntegrationUserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Integrations_IntegrationUserModelId",
                table: "Servers",
                column: "IntegrationUserModelId",
                principalTable: "Integrations",
                principalColumn: "Id");
        }
    }
}
