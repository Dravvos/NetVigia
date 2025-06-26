using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class Maintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceModelId",
                table: "Servers",
                type: "uuid",
                nullable: true);

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
                name: "IX_Servers_MaintenanceModelId",
                table: "Servers",
                column: "MaintenanceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceServers_MaintenanceId",
                table: "MaintenanceServers",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceServers_ServerId",
                table: "MaintenanceServers",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Maintenances_MaintenanceModelId",
                table: "Servers",
                column: "MaintenanceModelId",
                principalTable: "Maintenances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Maintenances_MaintenanceModelId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "MaintenanceServers");

            migrationBuilder.DropTable(
                name: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Servers_MaintenanceModelId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "MaintenanceModelId",
                table: "Servers");
        }
    }
}
