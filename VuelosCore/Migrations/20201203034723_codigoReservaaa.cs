using Microsoft.EntityFrameworkCore.Migrations;

namespace VuelosCore.Migrations
{
    public partial class codigoReservaaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoReserva",
                table: "ReservaVuelos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoReserva",
                table: "ReservaVuelos");
        }
    }
}
