using SpotifyExplode;
using YoutubeExplode;
using JukeboxEngine.WebSocket;
using JukeboxEngine.Interfaces;
using JukeboxEngine.WebSocket.Packets;

namespace JukeboxEngine.Events;

public class DownloadEvent : IEventHandler
{
  private readonly WebSocketClient _client;
  private readonly WebSocketService _server;

  private readonly YoutubeClient youtube = new();
  private readonly SpotifyClient spotify = new();

  private async void Process(dynamic data)
  {
    if (!_client.handshaked)
      return;

    string id = data["Id"];
    string platform = data["Platform"];

    if (data is null || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(platform))
      return;

    switch (platform)
    {
      case "youtube":
        {
          var info = await youtube.Videos.GetAsync(id);

          if (info is not null)
          {
            bool downloaded = Core.Instance.MediaDownloader!.GetIfDownloadExists(info.Title, info.Author.ChannelTitle);

            if (!downloaded)
            {
              var response = new DownloadPacket(new
              {
                Id = id,
                State = "Downloading"
              });
              await _server.SendMessage(response.ToJson(), _client);

              var download = await Core.Instance.MediaDownloader.Download(info.Author.ChannelTitle, info.Title, info.Url);
              if (download.IsDownloaded)
              {
                Core.Instance.Player!.Playlist.AddTrack(download);

                response.Response = new { Id = id, State = "Downloaded" };
                await _server.SendMessage(response.ToJson(), _client);
              }
            }
            else
            {
              var response = new DownloadPacket(new
              {
                Id = id,
                State = "Already_Downloaded"
              });

              await _server.SendMessage(response.ToJson(), _client);
            }
          }
          break;
        }

      case "spotify":
        {
          var info = await spotify.Tracks.GetAsync(id);

          if (info is not null)
          {
            var artist = info.Artists.Count > 1
              ? string.Join(", ", info.Artists.Select(artist => artist.Name))
              : info.Artists[0].Name;

            bool downloaded = Core.Instance.MediaDownloader!.GetIfDownloadExists(info.Title, artist);

            if (!downloaded)
            {
              var response = new DownloadPacket(new
              {
                Id = id,
                State = "Downloading"
              });
              await _server.SendMessage(response.ToJson(), _client);

              var download = await Core.Instance.MediaDownloader.Download(artist, info.Title, info.Url);
              if (download.IsDownloaded)
              {
                Core.Instance.Player!.Playlist.AddTrack(download);

                response.Response = new { Id = id, State = "Downloaded" };
                await _server.SendMessage(response.ToJson(), _client);
              }
            }
            else
            {
              var response = new DownloadPacket(new
              {
                Id = id,
                State = "Already_Downloaded"
              });
              await _server.SendMessage(response.ToJson(), _client);
            }
          }
          break;
        }

      default:
        {
          var response = new DownloadPacket(new
          {
            Id = id,
            State = "Platform_Not_Supported"
          });
          await _server.SendMessage(response.ToJson(), _client);
          break;
        }
    }
  }

  public void Handle(dynamic data)
  {
    Process(data);
  }

  public DownloadEvent(WebSocketClient client, WebSocketService server)
  {
    _client = client;
    _server = server;
  }
}

// var search = await Core.Instance.MediaSearch.Search(platform, query);
// if (search is not null)
// {
//   var track = search.FirstOrDefault();
//   if (track is not null)
//   {
//     string title = track.Title;
//     string author = track.Artists.Count > 1
//       ? string.Join(", ", track.Artists.Select(artist => artist.Name))
//       : track.Artists[0].Name;
//     bool downloaded = Core.Instance.MediaDownloader.GetIfDownloadExists(title, author);
//     JsonPacket response;
//     if (!downloaded)
//     {
//       Logger.Log(ELogLevel.Info, $"Downloading '{author} - {title}'..");
//       response = new JsonPacket("Download", new
//       {
//         Loading = true,
//         Message = $"Track '{author} - {title}' downloading"
//       });
//       _server.SendMessage(response.ToJson(), _socket);
//       var download = await Core.Instance.MediaDownloader.Download(author, title, track.Url);
//       if (download.IsDownloaded)
//       {
//         response = new JsonPacket("Download", new
//         {
//           Success = true,
//           Message = $"Track '{author} - {title}' downloaded successfully"
//         });
//         _server.SendMessage(response.ToJson(), _socket);
//         Logger.Log(ELogLevel.Info, $"Downloaded '{author} - {title}' successfully");
//         Core.Instance.Player.Playlist.AddTrack(download);
//       }
//     }
//     else
//     {
//       var check1 = Core.Instance.Player.Playlist.GetTrack(title, author);
//       if (check1 is not null)
//       {
//         response = new JsonPacket("Download", new
//         {
//           Success = false,
//           Message = $"Track '{author} - {title}' is already in playlist"
//         });
//         _server.SendMessage(response.ToJson(), _socket);
//         return false;
//       }
//       // if something buggy will happen then this is like kinda
//       var song = Core.Instance.MediaDownloader.GetDownloadedSong(title, author);
//       if (song is not null)
//       {
//         response = new JsonPacket("Download", new
//         {
//           Success = true,
//           Message = $"Track '{author} - {title}' added to playlist"
//         });
//         Core.Instance.Player.Playlist.AddTrack(song);
//         _server.SendMessage(response.ToJson(), _socket);
//       }
//     }
//   }
// }