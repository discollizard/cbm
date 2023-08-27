using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteCobmais.Migrations
{
    /// <inheritdoc />
    public partial class updateDividaIdLogField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogConsultas_Contratos_DividaId",
                table: "LogConsultas");

            migrationBuilder.DropIndex(
                name: "IX_LogConsultas_DividaId",
                table: "LogConsultas");

            migrationBuilder.DropColumn(
                name: "DividaId",
                table: "LogConsultas");

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "LogConsultas",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LogConsultas_ContratoId",
                table: "LogConsultas",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogConsultas_Contratos_ContratoId",
                table: "LogConsultas",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogConsultas_Contratos_ContratoId",
                table: "LogConsultas");

            migrationBuilder.DropIndex(
                name: "IX_LogConsultas_ContratoId",
                table: "LogConsultas");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "LogConsultas");

            migrationBuilder.AddColumn<int>(
                name: "DividaId",
                table: "LogConsultas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LogConsultas_DividaId",
                table: "LogConsultas",
                column: "DividaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogConsultas_Contratos_DividaId",
                table: "LogConsultas",
                column: "DividaId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
