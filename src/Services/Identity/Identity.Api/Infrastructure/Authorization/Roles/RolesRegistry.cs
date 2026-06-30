namespace Identity.Api.Infrastructure.Authorization.Roles;

public class RolesRegistry
{
    public static readonly RoleDefinition SuperAdmin = new("SuperAdmin", "مدیر کل سامانه", "دسترسی کامل به تمام امکانات سامانه");
    public static readonly RoleDefinition UniversityPresident = new("UniversityPresident", "رئیس دانشگاه", "مدیریت و نظارت در سطح کل دانشگاه");
    public static readonly RoleDefinition EducationManager = new("EducationManager", "مدیر آموزش", "مدیریت امور آموزشی دانشگاه");
    public static readonly RoleDefinition DepartmentManager = new("DepartmentManager", "مدیر گروه", "مدیریت گروه آموزشی");
    public static readonly RoleDefinition DepartmentExpert = new("DepartmentExpert", "کارشناس گروه", "انجام امور اجرایی و آموزشی گروه");
    public static readonly RoleDefinition Professor = new("Professor", "استاد", "عضو هیئت علمی و مدرس");
    public static readonly RoleDefinition Student = new("Student", "دانشجو", "دانشجوی دانشگاه");
    public static readonly RoleDefinition Unassigned = new("Unassigned", "بدون نقش", "کاربری که هنوز هیچ نقش عملیاتی به او اختصاص داده نشده است");

    public static readonly IReadOnlyList<RoleDefinition> All =
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