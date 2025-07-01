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
            migrationBuilder.AddColumn<Guid>(
                name: "IntegrationUserModelId",
                table: "Servers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IntegrationServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntegrationServers_Integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "Integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IntegrationServers_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IntegrationUserModelId",
                table: "Servers",
                column: "IntegrationUserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationServers_IntegrationId",
                table: "IntegrationServers",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationServers_ServerId",
                table: "IntegrationServers",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Integrations_IntegrationUserModelId",
                table: "Servers",
                column: "IntegrationUserModelId",
                principalTable: "Integrations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Integrations_IntegrationUserModelId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "IntegrationServers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_IntegrationUserModelId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "IntegrationUserModelId",
                table: "Servers");
        }
    }
}
