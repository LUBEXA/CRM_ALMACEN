using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargoSolicitudPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolicitudPedidoId",
                table: "Cargos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_SolicitudPedidoId",
                table: "Cargos",
                column: "SolicitudPedidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargos_Solicitudes_SolicitudPedidoId",
                table: "Cargos",
                column: "SolicitudPedidoId",
                principalTable: "Solicitudes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargos_Solicitudes_SolicitudPedidoId",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_SolicitudPedidoId",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "SolicitudPedidoId",
                table: "Cargos");
        }
    }
}
