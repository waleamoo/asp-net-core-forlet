using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLet.Migrations
{
    public partial class changedfeature2slug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Rentals",
                newName: "slug");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Rentals",
                newName: "Features");
        }
    }
}
