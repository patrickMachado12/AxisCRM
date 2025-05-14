using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AxisCRM.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriarEntidadeParecer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parecer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "VARCHAR", nullable: false),
                    PessoaContato = table.Column<string>(type: "VARCHAR", maxLength: 20, nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    IdAtendimento = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parecer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parecer_atendimento_IdAtendimento",
                        column: x => x.IdAtendimento,
                        principalTable: "atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parecer_usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_parecer_IdAtendimento",
                table: "parecer",
                column: "IdAtendimento");

            migrationBuilder.CreateIndex(
                name: "IX_parecer_IdUsuario",
                table: "parecer",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parecer");
        }
    }
}
