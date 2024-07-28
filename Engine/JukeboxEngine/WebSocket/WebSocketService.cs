using Fleck;
using System.Net;
using System.Net.Sockets;
using JukeboxEngine.WebSocket.Packets;
using JukeboxEngine.Enums;

namespace JukeboxEngine.WebSocket;

public class WebSocketService : WebSocketServer
{
  #region IS_PORT_AVAILABLE
  private static readonly int basePort = 5656;

  private static string GetAvailablePort()
  {
    for (int port = basePort; port <= basePort + 100; port++)
      if (IsPortAvailable(port))
      {
#if DEBUG
        return $"ws://{IPAddress.Loopback}:{port}";
#endif

#if RELEASE
        return $"ws://{IPAddress.Any}:{port}";
#endif
      }

    throw new Exception("No available port found");
  }

  private static bool IsPortAvailable(int port)
  {
    using (var client = new TcpClient())
    {
      try
      {
        client.Connect(IPAddress.Any, port);
        return false;
      }
      catch
      {
        return true;
      }
    }
  }
  #endregion

  private readonly List<WebSocketClient> _clients = new List<WebSocketClient>();
  private async Task UpdatePlayerState()
  {
    StatePacket state;

    while (true)
    {
      if (_clients.Count == 0) continue;

      try
      {
        state = new();

        if (state.Response is not null) await SendMessage(state.ToJson());
      }
      catch {
      }
      await Task.Delay(500);
    }
  }

  private void LogFleskAction(LogLevel level, string message, Exception exception)
  {
    if (exception is not null)
    {
      Logger.Log(ELogLevel.Error, $"WSS exception: {exception.Message}");
      return;
    }
  }

  private void Initialize()
  {
    FleckLog.LogAction = LogFleskAction;

    ListenerSocket.NoDelay = true;
    RestartAfterListenError = true;

    Task.Run(UpdatePlayerState);

    try
    {
      Start(socket => new WebSocketClient(this, socket));
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"WebSocket API Backend initializing failed: {ex.Message}");
      return;
    }

    Logger.Log(ELogLevel.Info, $"WebSocket API Backend listening on {Port}");
  }

  public WebSocketService() : base(GetAvailablePort())
  {
    Logger.Log(ELogLevel.Debug, "WebSocketService(): constructor");
    Initialize();
  }

  public async Task SendMessage(string message, WebSocketClient? target = null)
  {
    if (target is not null && target.handshaked && target.socket.IsAvailable)
      target.socket?.Send(message);
    else
    {
      foreach (var client in _clients)
      {
        if (client.socket.IsAvailable && client.handshaked)
          await Task.Run(() => client.socket.Send(message));
      }
    }
  }

  public void AddClient(WebSocketClient client)
  {
    if (!_clients.Contains(client))
      _clients.Add(client);
  }

  public void RemoveClient(WebSocketClient client)
  {
    if (_clients.Contains(client))
      _clients.Remove(client);
  }
}