using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessorIdentityProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmploymentType",
                table: "Professors",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "AcademicRank",
                table: "Professors",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "IdentityProvisioningFailureReason",
                table: "Professors",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityProvisioningStatus",
                table: "Professors",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityProvisioningFailureReason",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "IdentityProvisioningStatus",
                table: "Professors");

            migrationBuilder.AlterColumn<string>(
                name: "EmploymentType",
                table: "Professors",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicRank",
                table: "Professors",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
