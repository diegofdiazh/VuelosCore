using Microsoft.EntityFrameworkCore.Migrations;

namespace VuelosCore.Migrations
{
    public partial class reserva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservaVuelos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoVuelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaVuelos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservaVuelos");
        }
    }
}
