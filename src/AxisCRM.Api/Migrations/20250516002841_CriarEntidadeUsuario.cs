using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AxisCRM.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriarEntidadeUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: false),
                    CpfCnpj = table.Column<string>(type: "VARCHAR", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "VARCHAR", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: false),
                    Observacao = table.Column<string>(type: "VARCHAR", maxLength: 1000, nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TipoPessoa = table.Column<int>(type: "integer", nullable: false),
                    DataExclusao = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataExclusao = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Perfil = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "atendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCliente = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Assunto = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: false),
                    Historico = table.Column<string>(type: "text", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_atendimento_cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_atendimento_usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parecer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAtendimento = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR", nullable: false),
                    PessoaContato = table.Column<string>(type: "VARCHAR", maxLength: 20, nullable: false),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                name: "IX_atendimento_IdCliente",
                table: "atendimento",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_atendimento_IdUsuario",
                table: "atendimento",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_CpfCnpj",
                table: "cliente",
                column: "CpfCnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parecer_IdAtendimento",
                table: "parecer",
                column: "IdAtendimento");

            migrationBuilder.CreateIndex(
                name: "IX_parecer_IdUsuario",
                table: "parecer",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Email",
                table: "usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parecer");

            migrationBuilder.DropTable(
                name: "atendimento");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
