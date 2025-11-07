using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionMedicineDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dosage",
                table: "PrescriptionMedicines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "PrescriptionMedicines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "PrescriptionMedicines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dosage",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "PrescriptionMedicines");
        }
    }
}
