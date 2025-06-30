using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class NetVigiaDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maintenances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TabelaGeral",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelaGeral", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TabelaGeralItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TabelaGeralId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sigla = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelaGeralItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabelaGeralItem_TabelaGeral_TabelaGeralId",
                        column: x => x.TabelaGeralId,
                        principalTable: "TabelaGeral",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Integrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdTGIntegrationMethod = table.Column<Guid>(type: "uuid", nullable: false),
                    IntegrationName = table.Column<string>(type: "text", nullable: false),
                    IntegrationEndpoint = table.Column<string>(type: "text", nullable: false),
                    IdTGSendNotification = table.Column<Guid>(type: "uuid", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Integrations_TabelaGeralItem_IdTGIntegrationMethod",
                        column: x => x.IdTGIntegrationMethod,
                        principalTable: "TabelaGeralItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Integrations_TabelaGeralItem_IdTGSendNotification",
                        column: x => x.IdTGSendNotification,
                        principalTable: "TabelaGeralItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    URL = table.Column<string>(type: "text", nullable: false),
                    CheckIntervalSeconds = table.Column<int>(type: "integer", nullable: false),
                    ExpectedStatusCode = table.Column<int>(type: "integer", nullable: false),
                    ExpectedConten = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    TimeoutInSeconds = table.Column<int>(type: "integer", nullable: false),
                    IdTGMonitoringType = table.Column<Guid>(name: "IdTGMonitoringType ", type: "uuid", nullable: false),
                    IdTGHTTPMethod = table.Column<Guid>(type: "uuid", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaintenanceModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servers_Maintenances_MaintenanceModelId",
                        column: x => x.MaintenanceModelId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Servers_TabelaGeralItem_IdTGHTTPMethod",
                        column: x => x.IdTGHTTPMethod,
                        principalTable: "TabelaGeralItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Servers_TabelaGeralItem_IdTGMonitoringType ",
                        column: x => x.IdTGMonitoringType,
                        principalTable: "TabelaGeralItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaintenanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceServers_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceServers_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_IdTGIntegrationMethod",
                table: "Integrations",
                column: "IdTGIntegrationMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_IdTGSendNotification",
                table: "Integrations",
                column: "IdTGSendNotification");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceServers_MaintenanceId",
                table: "MaintenanceServers",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceServers_ServerId",
                table: "MaintenanceServers",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IdTGHTTPMethod",
                table: "Servers",
                column: "IdTGHTTPMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IdTGMonitoringType ",
                table: "Servers",
                column: "IdTGMonitoringType ");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_MaintenanceModelId",
                table: "Servers",
                column: "MaintenanceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TabelaGeralItem_TabelaGeralId",
                table: "TabelaGeralItem",
                column: "TabelaGeralId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Integrations");

            migrationBuilder.DropTable(
                name: "MaintenanceServers");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Maintenances");

            migrationBuilder.DropTable(
                name: "TabelaGeralItem");

            migrationBuilder.DropTable(
                name: "TabelaGeral");
        }
    }
}
