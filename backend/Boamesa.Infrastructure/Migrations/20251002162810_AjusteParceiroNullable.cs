using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boamesa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteParceiroNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Parceiros_ParceiroAppId",
                table: "Atendimentos");

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "ItensCardapio",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Parceiros_ParceiroAppId",
                table: "Atendimentos",
                column: "ParceiroAppId",
                principalTable: "Parceiros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Parceiros_ParceiroAppId",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "ItensCardapio");

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Parceiros_ParceiroAppId",
                table: "Atendimentos",
                column: "ParceiroAppId",
                principalTable: "Parceiros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
