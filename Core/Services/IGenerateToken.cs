using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IGenerateToken
    {
        string GetAndCreateToken(AppUser user, IList<string> rolename, IList<Claim> userClaims, bool isRefreshToken);
        Task AddRefreshTokenModel(CreateRefreshToken createRefreshToken);
        Task<CreateRefreshToken?> GetRefreshTokenModel(string refreshToken);
    }
}
