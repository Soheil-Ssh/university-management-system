using CentralOrganization.Api.Domain.Employee.Errors;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Enums;
using System.Text.RegularExpressions;
using CentralOrganization.Api.Application.Abstractions;
using SharedKernel.Domain.Identifiers;

namespace CentralOrganization.Api.Features.Employees.v1.Create;

public static class Create
{
    public sealed record CreateEmployeeRequest(Guid UnitId,
        string FirstName,
        string LastName,
        string? FatherName,
        string NationalCode,
        DateOnly BirthDate,
        Gender Gender,
        string MobileNumber,
        string? PhoneNumber,
        string Email,
        string EducationField,
        string JobTitle,
        Guid ProfileImageFileId);

    public sealed record Command(Guid UnitId,
        string FirstName,
        string LastName,
        string? FatherName,
        string NationalCode,
        DateOnly BirthDate,
        Gender Gender,
        string MobileNumber,
        string? PhoneNumber,
        string Email,
        string EducationField,
        string JobTitle,
        Guid ProfileImageFileId) : ICommand<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UnitId).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.FatherName).MaximumLength(100);
            RuleFor(x => x.NationalCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.BirthDate).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
            RuleFor(x => x.Gender).IsInEnum();
            RuleFor(x => x.MobileNumber).NotEmpty().Matches(@"^(?:\+98|98|0)?(9\d{9})$", RegexOptions.Compiled);
            RuleFor(x => x.PhoneNumber).Matches(@"^(?:\+98|98|0)?([1-9]\d{1,2})(\d{8})$", RegexOptions.Compiled);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.EducationField).NotEmpty().MaximumLength(150);
            RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ProfileImageFileId).NotEmpty();
        }
    }

    public class Handler(IUnitRepository unitRepository,
        IEmployeeRepository employeeRepository,
        IFileValidatorClient fileValidatorClient,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var unitId = new UnitId(request.UnitId);
            var unit = await unitRepository.GetById(unitId, cancellationToken);
            if (unit is null)
                return UnitErrors.NotFound;

            if (!unit.IsActive)
                return UnitErrors.Inactive;

            var personnelCode = await employeeRepository.GetNextPersonnelCodeAsync(cancellationToken);
            var personnelCodeResult = PersonnelCode.Create(personnelCode);
            if (personnelCodeResult.IsFailure)
                return personnelCodeResult.Error;

            var personnelCodeExists = await employeeRepository
                .IsExistPersonnelCodeAsync(personnelCodeResult.Data, cancellationToken);
            if (personnelCodeExists)
                return EmployeeErrors.PersonnelCodeAlreadyExists;

            var nationalCodeResult = NationalCode.Create(request.NationalCode);
            if (nationalCodeResult.IsFailure)
                return nationalCodeResult.Error;

            var nationalCodeExists = await employeeRepository
                .IsExistNationalCodeAsync(nationalCodeResult.Data, cancellationToken);
            if (nationalCodeExists)
                return EmployeeErrors.NationalCodeAlreadyExists;

            var mobileNumberResult = MobileNumber.Create(request.MobileNumber);
            if (mobileNumberResult.IsFailure)
                return mobileNumberResult.Error;

            var mobileNumberExists = await employeeRepository
                .IsExistMobileNumberAsync(mobileNumberResult.Data, cancellationToken);
            if (mobileNumberExists)
                return EmployeeErrors.MobileNumberAlreadyExists;

            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            var emailExists = await employeeRepository
                .IsExistEmailAsync(emailResult.Data, cancellationToken);

            if (emailExists)
                return EmployeeErrors.EmailAlreadyExists;

            var fileExistsResult = await fileValidatorClient.ExistsAsync(new FileId(request.ProfileImageFileId), cancellationToken);
            if (fileExistsResult.IsFailure)
                return fileExistsResult.Error;

            if (!fileExistsResult.Data)
                return EmployeeErrors.PersonalImageFileNotFound.WithPath(nameof(request.ProfileImageFileId));

            var employeeResult = Employee.Create(
                unitId,
                personnelCodeResult.Data,
                request.FirstName,
                request.LastName,
                request.FatherName,
                request.NationalCode,
                request.BirthDate,
                request.Gender,
                request.MobileNumber,
                request.PhoneNumber,
                request.Email,
                request.EducationField,
                request.JobTitle,
                request.ProfileImageFileId);

            if (employeeResult.IsFailure)
                return employeeResult.Error;

            await employeeRepository.AddAsync(employeeResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return employeeResult.Data.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/employees", async (ISender sender, CreateEmployeeRequest request) =>
                {
                    var command = request.Adapt<Command>();
                    var result = await sender.Send(command);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.EmployeesCreate)
                .Version(app, 1.0)
                .WithName("CreateEmployee")
                .WithTags("Employees");
        }
    }
}