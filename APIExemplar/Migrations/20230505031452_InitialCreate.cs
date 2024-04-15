using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiexemplar.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "APIExemplar");

            migrationBuilder.CreateTable(
                name: "Exemplar",
                schema: "APIExemplar",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookState = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfAcquisition = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exemplar", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exemplar",
                schema: "APIExemplar");
        }
    }
}
