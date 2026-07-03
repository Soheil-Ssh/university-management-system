namespace Student.Api.Domain.Admission.Services;

public interface IRegistrationTokenGenerator
{
    string Generate(int bytes = 32);
}