using Microsoft.EntityFrameworkCore.Migrations;

namespace VuelosCore.Migrations
{
    public partial class codigoReserva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoReserva",
                table: "ReservaVuelos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
