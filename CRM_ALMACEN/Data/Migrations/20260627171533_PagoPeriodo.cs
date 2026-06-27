using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class PagoPeriodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Anio",
                table: "Pagos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mes",
                table: "Pagos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Asignar el periodo de los pagos existentes a partir de su fecha.
            migrationBuilder.Sql(
                "UPDATE \"Pagos\" SET \"Anio\" = EXTRACT(YEAR FROM \"Fecha\")::int, " +
                "\"Mes\" = EXTRACT(MONTH FROM \"Fecha\")::int;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anio",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Mes",
                table: "Pagos");
        }
    }
}
