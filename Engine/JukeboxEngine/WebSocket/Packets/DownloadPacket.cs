using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class DownloadPacket : JsonPacket
{
  public DownloadPacket(object data):
  base("Download")
  {
    Response = data;
  }
}