using TodoApi.Models;

namespace TodoApi.Services;

public interface IAuthService
{
    string GenerateToken(User user);
    Task<User?> Authenticate(string email, string password);
    Task<bool> Register(User user);
}