using System.Security.Cryptography;

namespace Application.Helpers;

public static class CredentialsHelper
{
    public static string GenerateClientId()
    {
        var guid = Guid.NewGuid();
        var now = DateTime.Now;
        var rnd = Random.Shared.Next();

        var str = guid.ToString() + now + rnd;
        str = Convert.ToBase64String(new byte[8]);

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
}