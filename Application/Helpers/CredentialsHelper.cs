using System.Security.Cryptography;

namespace Application.Helpers;

public static class CredentialsHelper
{
    public static string GenerateClientId()
    {
        var guid = Guid.NewGuid();
        var rnd = Random.Shared.Next();

        var str = guid.ToString() + rnd;

        return str;
    }
    
    public static string GenerateClientSecret()
    {
        RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider();
        var buffer = new byte[64];
        cryptoRandomDataGenerator.GetBytes(buffer);
        var uniq = Convert.ToBase64String(buffer);
        return uniq;
    }

    public static int GenerateCode()
    {
        return Random.Shared.Next(10000, 1000000);
    }
}