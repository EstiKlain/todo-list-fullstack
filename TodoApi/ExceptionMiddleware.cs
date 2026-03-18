using System.Net;
using System.Text.Json;

namespace TodoApi.Middlewares; // ודאי שה-Namespace מתאים לשם הפרויקט שלך

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // ממשיך את הבקשה לשלב הבא בשרשרת
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // אם קרתה שגיאה בכל שלב שהוא בשרשרת, היא נתפסת כאן
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        // קביעת קוד שגיאה 500 (שגיאת שרת כללית)
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "אופס! קרתה שגיאה פנימית בשרת.",
            Detailed = exception.Message // שימושי מאוד בפיתוח כדי לראות מה השתבש ב-React
        };

        // הפיכת האובייקט לטקסט בפורמט JSON ושליחתו ללקוח
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}