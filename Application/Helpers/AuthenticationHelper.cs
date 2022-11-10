using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace Application.Helpers;

public static class AuthenticationHelper
{
    private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
    
    private static byte[] GenerateSalt(int size)
    {
        var salt = new byte[size];
        rng.GetBytes(salt);
        return salt;
    }

    public static string GenerateHash(string password, string salt)
    {
        var salt1 = Convert.FromBase64String(salt);

        using var hashGenerator = new Rfc2898DeriveBytes(password, salt1);
        hashGenerator.IterationCount = 10101;
        var bytes = hashGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes);
    }

    public static void ProvideSaltAndHash(this User user)
    {
        var salt = GenerateSalt(24);
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = GenerateHash(user.PasswordHash, user.Salt);
    }
    
    public static string GenerateAccessToken(ClaimsIdentity subject)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("qwertyuiopASDFGHJKL1234567890");
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = subject,
            Expires = DateTime.Now.AddMinutes(60),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsIdentity AssembleClaimsIdentity(string code)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim("code", code),
        });
        return subject;
    }
}