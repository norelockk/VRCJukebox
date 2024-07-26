using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class SearchPacket : JsonPacket
{
  public SearchPacket(object data):
  base("Search")
  {
    Response = data;
  }
}