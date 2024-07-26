using Fleck;
using Newtonsoft.Json;
using JukeboxEngine.Events;
using JukeboxEngine.Interfaces;
using JukeboxEngine.Enums;
using JukeboxEngine.WebSocket.Packets;

namespace JukeboxEngine.WebSocket;

public class WebSocketClient
{
  private WebSocketService _server;
  private Dictionary<string, IEventHandler> _eventHandlers = new Dictionary<string, IEventHandler> { };

  public IWebSocketConnection socket;
  public bool handshaked = false;

  private protected async void InitializeEvents()
  {
    if (!handshaked)
      return;

    _eventHandlers.Add("Seek", new SeekEvent(this, _server));
    _eventHandlers.Add("Toggle", new ToggleEvent(this, _server));
    _eventHandlers.Add("Download", new DownloadEvent(this, _server));
    
    var response = new HandshakedPacket();
    var response2 = new StatePacket();

    await _server.SendMessage(response.ToJson(), this);
    await _server.SendMessage(response2.ToJson(), this);
  }

  public WebSocketClient(WebSocketService server, IWebSocketConnection sock)
  {
    socket            = sock;
    socket.OnOpen     += OnSocketOpen;
    socket.OnClose    += OnSocketClose;
    socket.OnMessage  += OnSocketMessage;

    _server = server;
  }

  private protected void OnSocketOpen()
  {
    _server.AddClient(this);
    Logger.Log(ELogLevel.Debug, $"+ {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
  }

  private protected void OnSocketClose()
  {
    _server.RemoveClient(this);
    Logger.Log(ELogLevel.Debug, $"- {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
  }

  private protected void OnSocketMessage(string message)
  {
    dynamic? data;

    try
    {
      data = message is string ? JsonConvert.DeserializeObject(message) : null;
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"Failed to deserialize event: {ex.Message}");
      return;
    }

    if (data is not null)
    {
      try
      {
        string eventType = data["Event"];
        if (!handshaked && eventType is "Handshake")
        {
          // TODO detect sent handshake key in order to get handshaked
          dynamic handshakeData = data["Data"];
          // if (handshakeData is null) return;
          
          handshaked = true;
          InitializeEvents();
          return;
        }

        dynamic eventData = data["Data"];

        if (_eventHandlers.TryGetValue(eventType, out IEventHandler handler))
        {
          Logger.Log(ELogLevel.Debug, $"Received event data: {message}");

          try
          {
            handler.Handle(eventData);
          }
          catch (Exception ex)
          {
            Logger.Log(ELogLevel.Error, $"Failed to handle event: {ex.Message}");
            return;
          }
        }
        else
        {
          Logger.Log(ELogLevel.Warning, $"Received unknown event data: {message}");
        }
      }
      catch (Exception ex)
      {
        Logger.Log(ELogLevel.Error, $"Error while processing message: {ex.Message}");
        return;
      }
    }
  }
}