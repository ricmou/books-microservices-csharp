using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apicategories.Migrations
{
    public partial class UpdateCatTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CatName",
                schema: "APICategories",
                table: "Categories",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "APICategories",
                table: "Categories",
                newName: "CatName");
        }
    }
}
