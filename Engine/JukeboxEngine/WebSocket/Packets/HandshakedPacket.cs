using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class HandshakedPacket : JsonPacket
{
  public HandshakedPacket(object? data = null):
  base("Handshaked")
  {
    Response = data!;
  }
}