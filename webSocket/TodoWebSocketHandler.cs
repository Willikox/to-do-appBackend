using System.Net.WebSockets;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class TodoWebSocketHandler
{
    private static readonly List<WebSocket> _sockets = new List<WebSocket>();
    private readonly IServiceScopeFactory _scopeFactory;

    public TodoWebSocketHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        _sockets.Add(webSocket);
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                _sockets.Remove(webSocket);
            }
            else 
            {
                await NotifyTaskChangeAsync();
            }
        }
    }

    public async Task NotifyTaskChangeAsync()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
            var tasks = await context.TodoItems.ToListAsync();
            var completedCount = tasks.Count(t => t.IsCompleted);
            var notCompletedCount = tasks.Count(t => !t.IsCompleted);

            var message = $"Tareas completadas: {completedCount}, No completadas: {notCompletedCount}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            var outputBuffer = new ArraySegment<byte>(encodedMessage);

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                     await socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }   
        }
    }               
}
        