using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDosageAndSideEffectsToMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dosage",
                table: "Medicines",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SideEffects",
                table: "Medicines",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dosage",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "SideEffects",
                table: "Medicines");
        }
    }
}
