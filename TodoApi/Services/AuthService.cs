using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;



namespace TodoApi.Services;

public class AuthService : IAuthService
{
    private readonly TododbContext _context;
    private readonly IConfiguration _config;

    public AuthService(TododbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> Register(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return false;

        // הצפנת הסיסמה לפני שמירתה במסד הנתונים
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> Authenticate(string email, string password)
    {
        // 1. נחפש את המשתמש במסד הנתונים לפי האימייל בלבד
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // 2. אם המשתמש לא קיים - נחזיר null
        if (user == null)
        {
            return null;
        }

        // 3. נבדוק אם הסיסמה שהוזנה מתאימה לסיסמה המוצפנת בבסיס הנתונים
        // הפונקציה Verify מקבלת טקסט רגיל (מהמשתמש) וטקסט מוצפן (מה-DB)
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isPasswordValid)
        {
            return null; // הסיסמה לא נכונה
        }

        // 4. אם הכל תקין, נחזיר את המשתמש
        return user;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60), // תוקף לשעה
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}