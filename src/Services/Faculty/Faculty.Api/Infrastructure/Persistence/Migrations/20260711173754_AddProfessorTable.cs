using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faculty.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    NationalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Specialization = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AcademicRank = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EmploymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EmploymentStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProfileImageFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdentityUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FatherName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Code",
                table: "Professors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Email",
                table: "Professors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_IdentityUserId",
                table: "Professors",
                column: "IdentityUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_MobileNumber",
                table: "Professors",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_NationalCode",
                table: "Professors",
                column: "NationalCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Professors");
        }
    }
}
