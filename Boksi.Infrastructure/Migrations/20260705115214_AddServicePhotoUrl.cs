using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boksi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Services",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Services");
        }
    }
}
