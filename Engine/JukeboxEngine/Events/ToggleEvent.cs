using Fleck;
using NAudio.Wave;
using JukeboxEngine.WebSocket;
using JukeboxEngine.Interfaces;
using JukeboxEngine.Models.Classes;
using JukeboxEngine.Enums;
using JukeboxEngine.WebSocket.Packets;

namespace JukeboxEngine.Events;

public class ToggleEvent : IEventHandler
{
  private readonly WebSocketClient _client;
  private readonly WebSocketService _server;

  private bool Process(dynamic data)
  {
    if (!_client.handshaked)
      return false;

    string action = data["Action"];

    if (data is null || action is null)
      return false;

    switch (action)
    {
      case "Play":
        {
          bool state = Core.Instance.Player.PlaybackState is PlaybackState.Playing;
          bool notStopped = Core.Instance.Player.PlaybackState is not PlaybackState.Stopped;

          Logger.Log(ELogLevel.Info, $"Player play state toggled to be {(!state ? "playing" : "paused")}");

          if (notStopped)
          {
            if (!state)
              Core.Instance.Player.Play();
            else
              Core.Instance.Player.Pause();
          }
          else
            Core.Instance.Player._Play();

          break;
        }

      default:
        {
          Logger.Log(ELogLevel.Warning, $"Unknown toggle action: {action}");
          return false;
        }
    }

    return true;
  }

  public ToggleEvent(WebSocketClient client, WebSocketService server)
  {
    _client = client;
    _server = server;
  }

  public async void Handle(dynamic data)
  {
    bool success = Process(data);

    if (success)
    {
      var response = new TogglePacket(new
      {
        Playing = Core.Instance.Player!.PlaybackState,
        Repeating = Core.Instance.Player!.Playlist.isRepeatMode
      });
      await _server.SendMessage(response.ToJson(), _client);
    }
  }
}