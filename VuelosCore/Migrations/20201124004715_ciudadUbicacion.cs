using Microsoft.EntityFrameworkCore.Migrations;

namespace VuelosCore.Migrations
{
    public partial class ciudadUbicacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CiudadUbicacion",
                table: "Aeropuertos",
                newName: "CiudadUbicacin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CiudadUbicacin",
                table: "Aeropuertos",
                newName: "CiudadUbicacion");
        }
    }
}
