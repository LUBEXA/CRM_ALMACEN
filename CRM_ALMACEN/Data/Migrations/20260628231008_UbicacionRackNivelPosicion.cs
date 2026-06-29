using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionRackNivelPosicion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Ubicaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Posicion",
                table: "Ubicaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rack",
                table: "Ubicaciones",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "Posicion",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "Rack",
                table: "Ubicaciones");
        }
    }
}
