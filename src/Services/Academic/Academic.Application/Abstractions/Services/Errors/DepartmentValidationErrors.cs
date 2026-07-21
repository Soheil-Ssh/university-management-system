using SharedKernel.Domain.Error;

namespace Academic.Application.Abstractions.Services.Errors;

public static class DepartmentValidationErrors
{
    public static readonly Error Unavailable = new("DepartmentValidationService.Unavailable", "The Faculty service is currently unavailable.", ErrorType.Failure);
}