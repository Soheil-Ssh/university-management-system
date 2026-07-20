using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TheoreticalCredits = table.Column<int>(type: "integer", nullable: false),
                    PracticalCredits = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.CheckConstraint("ck_courses_practical_credits", "\"PracticalCredits\" >= 0");
                    table.CheckConstraint("ck_courses_theoretical_credits", "\"TheoreticalCredits\" >= 0");
                    table.CheckConstraint("ck_courses_total_credits", "\"TheoreticalCredits\" + \"PracticalCredits\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrerequisiteCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => x.Id);
                    table.CheckConstraint("ck_course_prerequisites_not_self", "\"CourseId\" <> \"PrerequisiteCourseId\"");
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_PrerequisiteCourseId",
                        column: x => x.PrerequisiteCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Curricula",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    MajorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    MinimumRequiredCredits = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curricula", x => x.id);
                    table.CheckConstraint("ck_curricula_effective_dates", "\"EffectiveTo\" IS NULL OR \"EffectiveTo\" >= \"EffectiveFrom\"");
                    table.CheckConstraint("ck_curricula_minimum_required_credits", "\"MinimumRequiredCredits\" > 0");
                    table.ForeignKey(
                        name: "FK_Curricula_Majors_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Majors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurriculumCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurriculumId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    RequirementType = table.Column<int>(type: "integer", nullable: false),
                    SuggestedSemester = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumCourses", x => x.Id);
                    table.CheckConstraint("ck_curriculum_courses_display_order", "\"DisplayOrder\" IS NULL OR \"DisplayOrder\" > 0");
                    table.CheckConstraint("ck_curriculum_courses_suggested_semester", "\"SuggestedSemester\" > 0");
                    table.ForeignKey(
                        name: "FK_CurriculumCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurriculumCourses_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_CourseId_PrerequisiteCourseId",
                table: "CoursePrerequisites",
                columns: new[] { "CourseId", "PrerequisiteCourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_PrerequisiteCourseId",
                table: "CoursePrerequisites",
                column: "PrerequisiteCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Code",
                table: "Courses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId_IsActive",
                table: "Courses",
                columns: new[] { "DepartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_Code",
                table: "Curricula",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_MajorId",
                table: "Curricula",
                column: "MajorId",
                unique: true,
                filter: "\"Status\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_MajorId_Status",
                table: "Curricula",
                columns: new[] { "MajorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumCourses_CourseId",
                table: "CurriculumCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumCourses_CurriculumId_CourseId",
                table: "CurriculumCourses",
                columns: new[] { "CurriculumId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumCourses_CurriculumId_SuggestedSemester_DisplayOrd~",
                table: "CurriculumCourses",
                columns: new[] { "CurriculumId", "SuggestedSemester", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Majors_Code",
                table: "Majors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Majors_DepartmentId",
                table: "Majors",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Majors_DepartmentId_IsActive",
                table: "Majors",
                columns: new[] { "DepartmentId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "CurriculumCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Curricula");

            migrationBuilder.DropTable(
                name: "Majors");
        }
    }
}
