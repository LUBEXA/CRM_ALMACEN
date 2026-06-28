using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionEnEntrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Ubicaciones_UbicacionId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UbicacionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UbicacionId",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "Posiciones",
                table: "Entradas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UbicacionId",
                table: "Entradas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_UbicacionId",
                table: "Entradas",
                column: "UbicacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Ubicaciones_UbicacionId",
                table: "Entradas",
                column: "UbicacionId",
                principalTable: "Ubicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Ubicaciones_UbicacionId",
                table: "Entradas");

            migrationBuilder.DropIndex(
                name: "IX_Entradas_UbicacionId",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "Posiciones",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "UbicacionId",
                table: "Entradas");

            migrationBuilder.AddColumn<int>(
                name: "UbicacionId",
                table: "Productos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UbicacionId",
                table: "Productos",
                column: "UbicacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Ubicaciones_UbicacionId",
                table: "Productos",
                column: "UbicacionId",
                principalTable: "Ubicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
