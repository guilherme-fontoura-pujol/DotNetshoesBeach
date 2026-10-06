using DotNetshoesBeach.Application.Interfaces;
using BCrypt.Net;

namespace DotNetshoesBeach.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
    }
}