using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingCode = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    RegistrationToken = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Step = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicantContactInfo_HasApplicantContactInfo = table.Column<bool>(type: "boolean", nullable: true),
                    Address_AdditionalInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Address_BuildingNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address_Province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address_Street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address_Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Mobile = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ApplicantPersonalInfo_BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApplicantPersonalInfo_BirthPlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ApplicantPersonalInfo_Gender = table.Column<int>(type: "integer", nullable: true),
                    ApplicantPersonalInfo_HasApplicantPersonalInfo = table.Column<bool>(type: "boolean", nullable: true),
                    ApplicantPersonalInfo_IssuePlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ApplicantPersonalInfo_MaritalStatus = table.Column<int>(type: "integer", nullable: true),
                    EnFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NationalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PersonalImageFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    DiplomaInfo_Average = table.Column<decimal>(type: "numeric", nullable: true),
                    DiplomaInfo_Field = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DiplomaInfo_GraduationYear = table.Column<int>(type: "integer", nullable: true),
                    DiplomaInfo_HasDiplomaInfo = table.Column<bool>(type: "boolean", nullable: true),
                    EmergencyContact_HasEmergencyContact = table.Column<bool>(type: "boolean", nullable: true),
                    EmergencyContact_Relation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmergencyContactFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmergencyContactLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmergencyContactMobile = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    EntranceInfo_AdmissionMethod = table.Column<int>(type: "integer", nullable: true),
                    EntranceInfo_AdmissionType = table.Column<int>(type: "integer", nullable: true),
                    EntranceInfo_EntranceExamRank = table.Column<int>(type: "integer", nullable: true),
                    EntranceInfo_EntranceScore = table.Column<double>(type: "double precision", nullable: true),
                    EntranceInfo_HasEntranceInfo = table.Column<bool>(type: "boolean", nullable: true),
                    EntranceInfo_Quota = table.Column<int>(type: "integer", nullable: true),
                    FatherInfo_HasFatherInfo = table.Column<bool>(type: "boolean", nullable: true),
                    FatherFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FatherLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FatherMobile = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    FatherNationalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    MotherInfo_HasMotherInfo = table.Column<bool>(type: "boolean", nullable: true),
                    MotherFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MotherLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MotherMobile = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    MotherNationalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AdmissionRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionAttachments_AdmissionRequests_AdmissionRequestId",
                        column: x => x.AdmissionRequestId,
                        principalTable: "AdmissionRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionAttachments_AdmissionRequestId",
                table: "AdmissionAttachments",
                column: "AdmissionRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionRequests_RegistrationToken",
                table: "AdmissionRequests",
                column: "RegistrationToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionRequests_TrackingCode",
                table: "AdmissionRequests",
                column: "TrackingCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionAttachments");

            migrationBuilder.DropTable(
                name: "AdmissionRequests");
        }
    }
}
