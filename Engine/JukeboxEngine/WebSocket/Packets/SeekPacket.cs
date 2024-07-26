using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class SeekPacket : JsonPacket
{
  public SeekPacket(object? data = null):
  base("Seek")
  {
    Response = data!;
  }
}