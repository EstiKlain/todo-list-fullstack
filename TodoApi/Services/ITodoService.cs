using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<List<Item>> GetAllItemsAsync(int userId);
    Task<Item?> GetItemByIdAsync(int id, int userId);
    Task AddItemAsync(Item newItem);
    Task UpdateItemAsync(Item item);
    Task DeleteItemAsync(int id, int userId);
}