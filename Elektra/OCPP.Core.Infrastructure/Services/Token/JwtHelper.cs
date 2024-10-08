using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
namespace OCPP.Core.Infrastructure.Services.Token;



public class JwtHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtHelper()
    {
        _secretKey = "deme_bosh_sheydir_mehebbet_ay_biraaat_0910";
        _issuer = "www.elektra.pp.az";
        _audience = "www.elektra.pp.az";
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
