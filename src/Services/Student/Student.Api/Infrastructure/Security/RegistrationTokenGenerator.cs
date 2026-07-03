using System.Security.Cryptography;

namespace Student.Api.Infrastructure.Security;

public class RegistrationTokenGenerator : IRegistrationTokenGenerator
{
    public string Generate(int bytes = 32)
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes));
}