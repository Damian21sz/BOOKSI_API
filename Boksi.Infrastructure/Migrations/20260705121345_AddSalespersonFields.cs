using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boksi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalespersonFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalespersonId",
                table: "Salons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentage",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalespersonId",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "CommissionPercentage",
                table: "AspNetUsers");
        }
    }
}
