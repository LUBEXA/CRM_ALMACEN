using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_ALMACEN.Data.Migrations
{
    /// <inheritdoc />
    public partial class PerfilCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConstanciaFiscalArchivo",
                table: "Clientes",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Correo2",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoArchivo",
                table: "Clientes",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Representante",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereFactura",
                table: "Clientes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Telefono2",
                table: "Clientes",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstanciaFiscalArchivo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Correo2",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "LogoArchivo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Representante",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RequiereFactura",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Telefono2",
                table: "Clientes");
        }
    }
}
