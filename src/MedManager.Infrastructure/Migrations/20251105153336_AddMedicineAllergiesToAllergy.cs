using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineAllergiesToAllergy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllergyId1",
                table: "MedicineAllergies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineAllergies_AllergyId1",
                table: "MedicineAllergies",
                column: "AllergyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineAllergies_Allergies_AllergyId1",
                table: "MedicineAllergies",
                column: "AllergyId1",
                principalTable: "Allergies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineAllergies_Allergies_AllergyId1",
                table: "MedicineAllergies");

            migrationBuilder.DropIndex(
                name: "IX_MedicineAllergies_AllergyId1",
                table: "MedicineAllergies");

            migrationBuilder.DropColumn(
                name: "AllergyId1",
                table: "MedicineAllergies");
        }
    }
}
