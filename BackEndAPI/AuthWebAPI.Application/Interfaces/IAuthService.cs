using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;

namespace AuthWebAPI.API.Controllers
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto userDto);
        Task<TokenRegenerateDto?> LoginAsync(UserDto userDto);
        Task<TokenRegenerateDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshtoken);
    }
}