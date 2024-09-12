using System.Data;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TodoItem>> GetTodoItems()
    {
        return _context.TodoItems.ToList();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodoItem(TodoItem item)
    {
        item.CreatedDate = DateTime.UtcNow;
        item.IsCompleted = false;
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();
        
        var handler = HttpContext.RequestServices.GetRequiredService<TodoWebSocketHandler>();
        await handler.NotifyTaskChangeAsync();
        return CreatedAtAction(nameof(GetTodoItems), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> CompleteTodoItem(int id)
    {
        var todoItem = _context.TodoItems.Find(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.IsCompleted = !todoItem.IsCompleted;
        await _context.SaveChangesAsync();
        
        var handler = HttpContext.RequestServices.GetRequiredService<TodoWebSocketHandler>();
        await handler.NotifyTaskChangeAsync();

        return NoContent();
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] TodoItem updatedItem)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();                                
        }

        todoItem.Title = updatedItem.Title;
        todoItem.Description = updatedItem.Description;
        todoItem.CreatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var handler = HttpContext.RequestServices.GetRequiredService<TodoWebSocketHandler>();
        await handler.NotifyTaskChangeAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        var todoItem = _context.TodoItems.Find(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        var handler = HttpContext.RequestServices.GetRequiredService<TodoWebSocketHandler>();
        await handler.NotifyTaskChangeAsync();

        return NoContent();
    }
}