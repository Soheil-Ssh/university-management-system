using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexesInDepartmentProfessorAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepartmentProfessorAssignments_DepartmentId",
                table: "DepartmentProfessorAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentProfessorAssignments_ProfessorId",
                table: "DepartmentProfessorAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentProfessorAssignments_ProfessorId",
                table: "DepartmentProfessorAssignments",
                column: "ProfessorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepartmentProfessorAssignments_ProfessorId",
                table: "DepartmentProfessorAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentProfessorAssignments_DepartmentId",
                table: "DepartmentProfessorAssignments",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentProfessorAssignments_ProfessorId",
                table: "DepartmentProfessorAssignments",
                column: "ProfessorId",
                unique: true);
        }
    }
}
