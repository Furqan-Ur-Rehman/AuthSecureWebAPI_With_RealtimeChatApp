using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthWebAPI.API.Controllers;
using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
using AuthWebAPI.Persistance.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthWebAPI.Infrastructure.Services
{
    public class AuthService(IConfiguration configuration, WebAPIDbContext context) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserDto userdto)
        {
            if (await context.Users.AnyAsync(u => u.Username == userdto.Username))
                return null;
            if (await context.Users.AnyAsync(u => u.Email == userdto.Email))
                return null;
            if (await context.Users.AnyAsync(u => u.PasswordHash == userdto.Password))
                return null;
            var user = new User();
            user.Username = userdto.Username;
            user.Email = userdto.Email;
            user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, userdto.Password);
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public async Task<TokenRegenerateDto?> LoginAsync(UserDto userdto)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Email == userdto.Email);
            if (user is null)
                return null;

            if (new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, userdto.Password) == PasswordVerificationResult.Failed)
                return null;
            var token = new TokenRegenerateDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await ReGenerateExpireToken(user)
            };
            return token;
        }

        private async Task<string> ReGenerateExpireToken(User user)
        {
            var randomnumber = new byte[32];
            using var rang = RandomNumberGenerator.Create();
            rang.GetBytes(randomnumber);
            var RefreshToken = Convert.ToBase64String(randomnumber);
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(1);
            await context.SaveChangesAsync();
            return RefreshToken;
        }
        //Create JWT Token
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Name, user.Roles)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<TokenRegenerateDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshtoken)
        {
            var user = await context.Users.FindAsync(refreshtoken.Id);
            if(user == null || user.RefreshToken != refreshtoken.RefreshToken
                || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }
            var token = new TokenRegenerateDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await ReGenerateExpireToken(user)
            };
            return token;
        }
    }
}

