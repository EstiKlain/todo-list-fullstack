using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Endpoints; // חשוב כדי לזהות את הניתובים
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using TodoApi.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // הגדרת המנעול ב-Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });

    // החלת המנעול על כל ה-Endpoints
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// 1. הגדרת מחרוזת חיבור
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

// 2. רישום ה-DbContext (MySQL)
builder.Services.AddDbContext<TododbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// 3. רישום שירות האימות (Dependency Injection)
builder.Services.AddScoped<IAuthService, AuthService>();

// 3. רישום השירות (Dependency Injection)
builder.Services.AddScoped<ITodoService, TodoService>();

// 4. הגדרת אימות JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

//4.הוספת שירות CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // מאפשר לכל כתובת לגשת
              .AllowAnyMethod()   // מאפשר את כל סוגי הפעולות (GET, POST וכו')
              .AllowAnyHeader();  // מאפשר את כל סוגי ה-Headers
    });
});

// 5. הוספת שירותי Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// 6. שימוש במידלוור לטיפול בשגיאות גלובליות
app.UseMiddleware<ExceptionMiddleware>();
// 5. שימוש ב-CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. שימוש באימות ובאוטוריזציה

app.UseAuthentication(); // בודק מי המשתמש לפי הטוקן
app.UseAuthorization();  // בודק אם מותר לו לבצע את הפעולה

// 7. שימוש בנתיבים שהפרדנו לקובץ אחר
app.MapTodoEndpoints();
app.MapAuthEndpoints();


app.Run();