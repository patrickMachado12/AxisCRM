using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxisCRM.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriarEntidadeAtendimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Assunto",
                table: "atendimento",
                type: "VARCHAR",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assunto",
                table: "atendimento");
        }
    }
}
