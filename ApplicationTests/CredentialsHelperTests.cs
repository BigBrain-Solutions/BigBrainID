using Application.Helpers;

namespace ApplicationTests;

public class CredentialsHelperTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CodeGeneration_Test()
    {
        var code = CredentialsHelper.GenerateCode();

        if (code is >= 10000 and <= 1000000)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void GenerateClientId_Test()
    {
        var cId = CredentialsHelper.GenerateClientId();
        var guid = cId[..36];
        var rn = cId.Substring(36, cId.Length - 36);
        var g = Guid.Empty;

        if (Guid.TryParse(guid, out g) && !string.IsNullOrEmpty(rn))
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }
}