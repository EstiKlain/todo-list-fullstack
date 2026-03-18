using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // 1. נתיב הרשמה
        app.MapPost("/register", async (User newUser, IAuthService authService) =>
        {
            var success = await authService.Register(newUser);
            if (!success)
                return Results.BadRequest(new { Message = "User already exists or invalid data" });

            var token = authService.GenerateToken(newUser);

            return Results.Ok(new
            {
                Message = "User registered successfully!",
                Token = token,
                User = new { newUser.FullName, newUser.Email }
            });
        });

        // 2. נתיב התחברות
        app.MapPost("/login", async (User loginData, IAuthService authService) =>
        {
            // בדיקת אימות (אימייל וסיסמה)
            var user = await authService.Authenticate(loginData.Email, loginData.Password);

            if (user == null)
                return Results.Unauthorized();

            // אם הכל תקין - נייצר טוקן ונשלח אותו לקליינט
            var token = authService.GenerateToken(user);
            return Results.Ok(new { Token = token, User = new { user.FullName, user.Email } });
        });
    }
}