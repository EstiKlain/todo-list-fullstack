using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly TododbContext _context;

    public TodoService(TododbContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetAllItemsAsync(int userId) => 
        await _context.Items.Where(i => i.UserId == userId).ToListAsync();

    public async Task<Item?> GetItemByIdAsync(int id, int userId) => 
        await _context.Items.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

    public async Task AddItemAsync(Item newItem)
    {
        if (newItem == null) throw new ArgumentNullException(nameof(newItem));
        // if (newItem.IsComplete==null)
        // {
        //     newItem.IsComplete = false; 
        // }
        _context.Items.Add(newItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Item item)
    {
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int id, int userId)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
        if (item != null)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}