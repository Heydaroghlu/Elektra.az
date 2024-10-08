using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OCPP.Core.Application.Abstractions;
using OCPP.Core.Application.DTOs.TokenDTO;
using OCPP.Core.Database;

namespace OCPP.Core.Infrastructure.Services.Token;

public class TokenHandler:ITokenHandler
{
    readonly IConfiguration _configuration;
    readonly UserManager<AppUser> _userManager;
    public TokenHandler(IConfiguration configuration,UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    public  TokenDTO CreateAccessToken(AppUser user,int hour)
    {
        TokenDTO token = new TokenDTO();
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        token.Expiration = DateTime.UtcNow.AddHours(hour);
        List<Claim> claims = CreateClaims(user,hour);
        JwtSecurityToken securityToken = new(
            claims:claims,
            audience: _configuration["Token:Audience"],
            issuer: _configuration["Token:Issuer"],
            expires: token.Expiration,
            notBefore: DateTime.UtcNow,
            signingCredentials:signingCredentials
        );
        JwtSecurityTokenHandler tokenHandler = new();
        token.AccessToken= tokenHandler.WriteToken(securityToken);

        return token;
    }
    public  List<Claim> CreateClaims(AppUser user,int minute)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name,user.UserName),
            new Claim("FullName",user.FullName),
            new Claim(ClaimTypes.Role,user.Type.ToString()),
            new Claim(ClaimTypes.Expiration,DateTime.UtcNow.AddHours(4).AddHours(minute).ToString())
        };
        var adminRoles = _userManager.GetRolesAsync(user).Result;
        var roleClaims = adminRoles.Select(x => new Claim(ClaimTypes.Role, x));
        claims.AddRange(roleClaims);

        return claims;
    }
}