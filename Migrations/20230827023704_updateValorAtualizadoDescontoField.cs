using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteCobmais.Migrations
{
    /// <inheritdoc />
    public partial class updateValorAtualizadoDescontoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ValorAtualizado",
                table: "LogConsultas",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "DescontoMaximo",
                table: "LogConsultas",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "ValorAtualizado",
                table: "LogConsultas",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "DescontoMaximo",
                table: "LogConsultas",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
