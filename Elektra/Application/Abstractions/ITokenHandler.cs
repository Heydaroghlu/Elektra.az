using System.Security.Claims;
using OCPP.Core.Application.DTOs.TokenDTO;
using OCPP.Core.Database;

namespace OCPP.Core.Application.Abstractions;

public interface ITokenHandler
{
    
    TokenDTO CreateAccessToken(AppUser user,int minute);
    List<Claim> CreateClaims(AppUser user,int minute);
}