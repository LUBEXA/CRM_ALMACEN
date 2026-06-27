using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargoPrecioUnitarioTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioUnitario",
                table: "Cargos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Cargos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Calcular el precio unitario de los cargos existentes a partir del importe.
            migrationBuilder.Sql(
                "UPDATE \"Cargos\" SET \"PrecioUnitario\" = " +
                "CASE WHEN \"Cantidad\" > 0 THEN \"Monto\" / \"Cantidad\" ELSE \"Monto\" END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioUnitario",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Cargos");
        }
    }
}
