using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.WebSocket.Packets;

public class StatePacket : JsonPacket
{
  public StatePacket() :
  base("State")
  {
    UpdateData();
  }

  private void UpdateData()
  {
    if (Response is null)
    {
      Track currentTrack = Core.Instance.Player.Playlist.GetCurrentTrack();
      if (currentTrack is null) currentTrack = new() { Id = "-1", Name = "N/A", Author = "N/A" };

      bool isMuted = Core.Instance.Player.Volume == 0;
      long currentLength = Core.Instance.Player.GetCurrentLength();
      long currentPosition = Core.Instance.Player.GetCurrentPosition();

      var state = new { Muted = isMuted, Volume = Core.Instance.Player.Volume, Playback = Core.Instance.Player.PlaybackState };
      var track = new { Id = currentTrack.Id, Title = currentTrack.Name, Author = currentTrack.Author };
      var progress = new { Length = currentLength, Position = currentPosition };

      Response = new
      {
        State = state,
        Track = track,
        Progress = progress
      };
    }
  }
}