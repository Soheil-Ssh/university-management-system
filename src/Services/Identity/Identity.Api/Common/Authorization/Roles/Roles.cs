namespace Identity.Api.Common.Authorization.Roles;

public class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string UniversityPresident = "UniversityPresident";
    public const string EducationManager = "EducationManager";
    public const string DepartmentManager = "DepartmentManager";
    public const string DepartmentExpert = "DepartmentExpert";
    public const string Professor = "Professor";
    public const string Student = "Student";
    public const string Unassigned = "Unassigned";

    public static readonly IReadOnlyList<string> All =
    [
        SuperAdmin,
        UniversityPresident,
        EducationManager,
        DepartmentManager,
        DepartmentExpert,
        Professor,
        Student,
        Unassigned
    ];
}