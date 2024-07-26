using NAudio.Wave;
using JukeboxEngine.Enums;
using JukeboxEngine.Utils;
using JukeboxEngine.WebSocket;
using JukeboxEngine.Interfaces;
using JukeboxEngine.Models.Classes;
using JukeboxEngine.WebSocket.Packets;

namespace JukeboxEngine.Events;

public class SeekEvent : IEventHandler
{
  private readonly WebSocketClient _client;
  private readonly WebSocketService _server;

  private bool Process(dynamic data)
  {
    if (!_client.handshaked)
      return false;

    string action = data["Action"];
    dynamic value = data["Value"];

    if (data is null || action is null || value is null)
      return false;

    switch (action)
    {
      case "Volume":
        {
          float volume = NumberUtils.MathFixture(Convert.ToSingle(value), 1);
          string display = $"{Math.Floor(NumberUtils.MathFixture(volume * 100, 100))}%";

          Core.Instance.Player!.Volume = volume;
          Logger.Log(ELogLevel.Info, $"Player volume set to {display}");
          break;
        }

      case "Position":
        {
          long length = Core.Instance.Player!.GetCurrentLength();
          double position = NumberUtils.MathFixture(Convert.ToSingle(value), length);

          Core.Instance.Player.CurrentAudio!.CurrentTime = TimeSpan.FromSeconds(position);
          Logger.Log(ELogLevel.Info, $"Player current time seek set to {position}s");
          break;
        }

      default:
        {
          Logger.Log(ELogLevel.Warning, $"Unknown seek action: {action}");
          return false;
        }
    }

    return true;
  }

  public SeekEvent(WebSocketClient client, WebSocketService server)
  {
    _client = client;
    _server = server;
  }

  public async void Handle(dynamic data)
  {
    bool success = Process(data);

    if (success)
    {
      var response = new SeekPacket(new
      {
        Volume = Core.Instance.Player!.Volume,
        Length = Core.Instance.Player.GetCurrentLength(),
        Position = Core.Instance.Player.GetCurrentPosition(),
      });
      await _server.SendMessage(response.ToJson(), _client);
    }
  }
}