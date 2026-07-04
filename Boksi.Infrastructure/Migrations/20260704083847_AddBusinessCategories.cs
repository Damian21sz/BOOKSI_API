using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boksi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BusinessCategoryId",
                table: "Salons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salons_BusinessCategoryId",
                table: "Salons",
                column: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Salons_BusinessCategories_BusinessCategoryId",
                table: "Salons",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salons_BusinessCategories_BusinessCategoryId",
                table: "Salons");

            migrationBuilder.DropTable(
                name: "BusinessCategories");

            migrationBuilder.DropIndex(
                name: "IX_Salons_BusinessCategoryId",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "BusinessCategoryId",
                table: "Salons");
        }
    }
}
