using TodoApi.Services;
using TodoApi.Models;
using System.Security.Claims;

namespace TodoApi.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/items").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, ITodoService service) =>
        {
            // שליפת ה-ID מהטוקן
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await service.GetAllItemsAsync(userId);
        });

        group.MapGet("/{id}", async (int id,ClaimsPrincipal user, ITodoService service) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await service.GetItemByIdAsync(id, userId) is Item item ? Results.Ok(item) : Results.NotFound(new { Message = $"Item with ID {id} not found." });
        });

        group.MapPost("/", async (Item newItem,ClaimsPrincipal user, ITodoService service) =>
        {

            if (string.IsNullOrWhiteSpace(newItem.Name))
                return Results.BadRequest(new { Error = "Task name cannot be empty." });
            // שליפת ה-ID מהטוקן והצבתו ב-UserId של הפריט החדש
            newItem.UserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await service.AddItemAsync(newItem);
            return Results.Created($"/items/{newItem.Id}", newItem);
        });

        group.MapPut("/{id}", async (int id, Item item,ClaimsPrincipal user, ITodoService service) =>
        {

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var existingItem = await service.GetItemByIdAsync(id, userId);

            if (existingItem == null)
                return Results.NotFound(new { Error = "Cannot update. Item not found." });

            // 1. עדכון שם: רק אם נשלח טקסט
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                existingItem.Name = item.Name;
            }

            // 2. עדכון סטטוס: רק אם נשלח ערך (לא null)
            if (item.IsComplete.HasValue)
            {
                existingItem.IsComplete = item.IsComplete;
            }

            // שאר השדות יישארו בדיוק כפי שנשלפו מהמסד (existingItem)
            await service.UpdateItemAsync(existingItem);

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id,ClaimsPrincipal user, ITodoService service) =>
        {

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var existingItem = await service.GetItemByIdAsync(id, userId);
            if (existingItem == null)
                return Results.NotFound(new { Error = "Cannot delete. Item not found." });

            await service.DeleteItemAsync(id, userId);
            return Results.NoContent();
        });
    }
}