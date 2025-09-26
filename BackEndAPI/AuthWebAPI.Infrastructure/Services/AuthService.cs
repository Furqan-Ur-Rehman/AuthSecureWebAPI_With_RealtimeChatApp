using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthWebAPI.API.Controllers;
using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
using AuthWebAPI.Persistance.Context;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthWebAPI.Infrastructure.Services
{
    public class AuthService(IConfiguration configuration, WebAPIDbContext context, IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserDto userdto)
        {
            //try
            //{
                var user = new User();

                if (await context.Users.AnyAsync(u => u.Username.Equals(userdto.Username)))
                    return null;
                if (await context.Users.AnyAsync(u => u.Email.Equals(userdto.Email)))
                    return null;

            //if (new PasswordHasher<User>()
            //    .VerifyHashedPassword(user, user.PasswordHash, userdto.Password) == PasswordVerificationResult.Success)
            //    return null;
                user.Username = userdto.Username;
                user.Email = userdto.Email;
                user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, userdto.Password);
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return user;
            //}
            //catch (Exception ex) 
            //{
            //    return ex.Message;
            //}


        }

        public async Task<TokenRegenerateDto?> LoginAsync(UserDto userdto)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals(userdto.Email));
            if (user is null)
                return null;

            if(user.Username.Equals(userdto.Username) && user.Email.Equals(userdto.Email))
            {
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
            else
            {
                return null;
            }
        }
        // Create Cookie Method
        private void SetRefreshCookie(string token, DateTime expiresAtUtc)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                  // can't be accessed by JS
                Secure = true,                    // only over HTTPS
                Expires = expiresAtUtc,           // expiry date
                SameSite = SameSiteMode.None,     // allow cross-site cookies
                Path = "/",                       // cookie valid for all routes
                Domain = "localhost"           // match your backend API domain
                
            };
            //Write Cookie into Http Response so that browser store it.
            httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        private async Task<string> ReGenerateExpireToken(User user)
        {
            var randomnumber = new byte[32];
            using var rang = RandomNumberGenerator.Create();
            rang.GetBytes(randomnumber);
            var PlainToken = Convert.ToBase64String(randomnumber);
            var hashToken = HashToken(PlainToken);


            var RefreshToken = new RefreshToken
            {
                TokenHash = hashToken,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                // Get Current Logged in IP Address and store in DB
                CreatedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                UserId = user.Id,
                
            };
            //user.CreatedHashTokenAt = DateTime.UtcNow;
            //user.HashTokenExpiry = DateTime.UtcNow.AddHours(1);
            //user.CreatedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            //user.ReplacedByTokenHash = null;

            // set cookie (HttpOnly, Secure)
            SetRefreshCookie(PlainToken, RefreshToken.ExpiresAtUtc);


            context.Add(RefreshToken);
            await context.SaveChangesAsync();
            return PlainToken;
        }

        //Hash Token Method
        private static string HashToken(string plaintoken)
        {
            using var sha = SHA256.Create(); // 1. Create SHA-256 instance/object
            var bytes = Encoding.UTF8.GetBytes(plaintoken); // 2. Convert plain token string into bytes
            var hash = sha.ComputeHash(bytes); // 3. Hash the bytes (32 bytes result)
            return Convert.ToBase64String(hash!); // 4. Convert hash into Base64 text
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

        public async Task<TokenRegenerateDto?> RefreshTokenAsync(string Plainrefreshtoken)
        {
            
            //Convert the incoming Refresh Token into Hash Token then validate
            using var sha256 = SHA256.Create();
            var hashBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(Plainrefreshtoken!));
            var ConvertedHashToken = Convert.ToBase64String(hashBytes!);

            //context.RefreshTokens.Include(rtoken => rtoken.User);
            //var DbRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == ConvertedHashToken);
            var DbRefreshToken = await context.RefreshTokens.Include(rtoken => rtoken.User).FirstOrDefaultAsync(rtoken => rtoken.TokenHash == ConvertedHashToken);
            if(DbRefreshToken == null || DbRefreshToken.Revoked || DbRefreshToken.TokenHash != ConvertedHashToken
                || DbRefreshToken.ExpiresAtUtc < DateTime.UtcNow)
            {
                return null;
            }
            

            var token = new TokenRegenerateDto
            {
                AccessToken = CreateToken(DbRefreshToken.User),
                RefreshToken = await ReGenerateExpireToken(DbRefreshToken.User)
            };
            // Rotation: revoke old token and create new refresh token
            //user.Revoked = true;
            //user.RevokedAtUtc = DateTime.UtcNow;
            //user.RevokedByIp = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress?.ToString();
            DbRefreshToken.ReplacedByTokenHash = HashToken(token.RefreshToken); 
            await context.SaveChangesAsync(); // above user saved into database

            // reset new cookie (HttpOnly, Secure)
            SetRefreshCookie(token.RefreshToken, DateTime.UtcNow.AddHours(1));
            return token;
        }

        public async Task<string> LogoutAsync(string PlainToken)
        {
            //Convert the incoming Refresh Token into Hash Token then validate
                using var sha256 = SHA256.Create();
                var hashBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(PlainToken!));
                var ConvertedHashToken = Convert.ToBase64String(hashBytes!);

            var DBHashToken = await context.RefreshTokens.FirstOrDefaultAsync(u => u.TokenHash == ConvertedHashToken);
            if (DBHashToken != null)
            {
                DBHashToken.Revoked = true;
                DBHashToken.RevokedAtUtc = DateTime.UtcNow;
                DBHashToken.RevokedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                await context.SaveChangesAsync();
            }
            return PlainToken;
        }
    }
}

