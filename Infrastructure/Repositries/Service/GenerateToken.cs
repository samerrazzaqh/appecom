using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Entities;
using Core.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositries.Service
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration configuration;
        private readonly AppDbContext dbContext;

        public GenerateToken(IConfiguration configuration, AppDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public string GetAndCreateToken(AppUser user, IList<string> rolename, IList<Claim> userClaims,
            bool isRefreshToken)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp ?? string.Empty),
            };
            claims.AddRange(rolename.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userClaims);
            
            string secret = configuration.GetValue<string>($"Jwt:{(isRefreshToken ? "RefreshTokenSecret" : "Secret")}");
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("Token secret is not configured");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: "doseHieu",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(isRefreshToken ? 24*60 : 30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task AddRefreshTokenModel(CreateRefreshToken refreshTokenModel)
        {
            await RemoveRefreshTokenByUserID(refreshTokenModel.UserId);
            await dbContext.CreateRefreshTokens.AddAsync(refreshTokenModel);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CreateRefreshToken?> GetRefreshTokenModel(string refreshToken)
        {
            return await dbContext.CreateRefreshTokens.Include(n => n.AppUser)
                .FirstOrDefaultAsync(n => n.RefreshToken == refreshToken);
        }

        public async Task RemoveRefreshTokenByUserID(string userID)
        {
            var refreshToken = await dbContext.CreateRefreshTokens
                .FirstOrDefaultAsync(n => n.UserId == userID);

            if (refreshToken != null)
            {
                dbContext.CreateRefreshTokens.Remove(refreshToken);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}