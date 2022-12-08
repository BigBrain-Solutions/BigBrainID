using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Helpers;
using Domain.Models;

namespace ApplicationTests;

public class AuthenticationHelperTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenerateSalt_Test()
    {
        const int size = 24;
        
        var salt = AuthenticationHelper.GenerateSaltMock(size);

        var saltString = Convert.ToBase64String(salt);
        
        var isNullOrEmpty = string.IsNullOrEmpty(saltString);

        if (isNullOrEmpty)
        {
            Assert.Fail("Salt string is null");
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void GenerateSalt_Test_WithZero_ExpectedEmpty()
    {
        const int size = 0;
        
        var salt = AuthenticationHelper.GenerateSaltMock(size);

        var saltString = Convert.ToBase64String(salt);

        var isNullOrEmpty = string.IsNullOrEmpty(saltString);

        if (isNullOrEmpty)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void GenerateHash_Test()
    {
        const string password = "ZAQ!2wsx";

        var salt = Convert.ToBase64String(AuthenticationHelper.GenerateSaltMock(24));
        var hash = AuthenticationHelper.GenerateHash(password, salt);
        
        var isNullOrEmpty = string.IsNullOrEmpty(hash);
        
        if (isNullOrEmpty)
        {
            Assert.Fail("PasswordHash is null");
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void ProvideHashAndSalt_Test()
    {
        const string password = "ZAQ!2wsx";
        
        var user = new User
        {
            PasswordHash = password
        };
        
        user.ProvideSaltAndHash();

        if (string.IsNullOrEmpty(user.Salt) && string.IsNullOrEmpty(user.PasswordHash))
        {
            Assert.Fail("Salt, Password was null");
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void AssembleClaimsIdentity_Test()
    {
        var userId = Guid.NewGuid();

        const string scope = "profile-read";
        
        var claims = AuthenticationHelper.AssembleClaimsIdentity(userId, scope);

        if (claims.HasClaim("id", userId.ToString()) && claims.HasClaim("scope", scope))
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }
    
    [Test]
    public void GenerateAccessToken_Test_Claims()
    {
        var userId = Guid.NewGuid();

        const string scope = "profile-read";
        
        var settings = new Settings
        {
            BearerKey = "ZXCVBNMASDFGHJKLQWERTYUIOP1234567890"
        };

        var claims = AuthenticationHelper.AssembleClaimsIdentity(userId, scope);

        var accessToken = AuthenticationHelper.GenerateAccessToken(settings, claims);
        
        var handler = new JwtSecurityTokenHandler();
        var accessTokenDecoded = handler.ReadJwtToken(accessToken);
        var decodedClaims = accessTokenDecoded.Claims;
        
        var claimsArr = decodedClaims as Claim[] ?? decodedClaims.ToArray();
        
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            claimsArr.ToArray()[0],
            claimsArr.ToArray()[1]
        });

        if (claimsIdentity.HasClaim("id", userId.ToString()) && claimsIdentity.HasClaim("scope", scope))
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void GenerateAccessToken_Test_Valid()
    {
        var userId = Guid.NewGuid();

        const string scope = "profile-read";
        
        var settings = new Settings
        {
            BearerKey = "ZXCVBNMASDFGHJKLQWERTYUIOP1234567890"
        };

        var claims = AuthenticationHelper.AssembleClaimsIdentity(userId, scope);

        var accessToken = AuthenticationHelper.GenerateAccessToken(settings, claims);
        
        var handler = new JwtSecurityTokenHandler();
        var accessTokenDecoded = handler.ReadJwtToken(accessToken);

        var validTo = accessTokenDecoded.ValidFrom + TimeSpan.FromHours(1);

        if (accessTokenDecoded.ValidTo == validTo)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }        
    }
}