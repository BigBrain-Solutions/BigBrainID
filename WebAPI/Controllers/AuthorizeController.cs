using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeController : ControllerBase
{
    [HttpGet]
    public IActionResult Callback(string clientId, string redirectUri, string scope)
    {
        // TODO: check if clientId is valid
        // TODO: check if redirectUri is on the list

        var code = "32131";
        
        redirectUri += $"?code={code}"; // TODO: Generate the code and save to database
        
        return Redirect(redirectUri);
    }

    [HttpPost("token")]
    public IActionResult GrantToken([FromBody] GrantTokenRequest request, [FromHeader] string clientId, [FromHeader] string clientSecret)
    {
        // TODO: Check if Code is valid
        // TODO: Check Grant Type
        
        // TODO: Generate Access Token and save it to database
        // TODO: Generate Refresh Token
        
        // TODO: Check for client id or client secret

        var accessToken = "test";
        var refreshToken = "test";
        
        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}