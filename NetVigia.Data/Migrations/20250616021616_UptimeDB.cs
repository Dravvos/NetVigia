using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetVigia.Data.Migrations
{
    /// <inheritdoc />
    public partial class UptimeDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servers_TabelaGeralItem_IdTGMonitoringType ",
                        column: x => x.IdTGMonitoringType,
                        principalTable: "TabelaGeralItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IdTGMonitoringType ",
                table: "Servers",
                column: "IdTGMonitoringType ");

            migrationBuilder.CreateIndex(
                name: "IX_TabelaGeralItem_TabelaGeralId",
                table: "TabelaGeralItem",
                column: "TabelaGeralId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "TabelaGeralItem");

            migrationBuilder.DropTable(
                name: "TabelaGeral");
        }
    }
}
