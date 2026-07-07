namespace CentralOrganization.Api.Application.Abstractions.Identity;

public sealed record CreateEmployeeUserRequest(string UserName, string Email, string Password);