using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class TogglePacket : JsonPacket
{
  public TogglePacket(object? data = null):
  base("Toggle")
  {
    Response = data!;
  }
}