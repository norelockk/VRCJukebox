using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class MessagePacket : JsonPacket
{
  public MessagePacket(object data):
  base("Message")
  {
    Response = data;
  }
}