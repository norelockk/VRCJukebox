// using WebSocketSharp.Server;
// using JukeboxEngine.Models.Enums;
// using JukeboxEngine.Models.Classes;

// namespace JukeboxEngine.Events;

// public class AddTrackEvent
// {
//   private WebSocketSessionManager _session;

//   public AddTrackEvent(WebSocketSessionManager session)
//   {
//     _session = session;
//   }

//   public async void Process(string url)
//   {
//     var trackInfo = await Core.Instance.MediaSearch.GetTrackInfoAsync(url);

//     if (trackInfo is not null)
//     {
//       string title = trackInfo.Title;
//       string author = trackInfo.Artists.Count > 1
//         ? string.Join(", ", trackInfo.Artists.Select(artist => artist.Name))
//         : trackInfo.Artists[0].Name;

//       bool downloaded = Core.Instance.MediaDownloader.GetIfDownloadExists(title, author);

//       if (!downloaded)
//       {
//         Logger.Log(LogLevel.Info, $"Downloading '{author} - {title}'..");

//         var response = new JsonPacket("AddTrack", new
//         {
//           Loading = true,
//           Message = $"Track '{author} - {title}' downloading"
//         });
//         _session.Broadcast(response.ToJson());

//         var download = await Core.Instance.MediaDownloader.Download(author, title, url);
//         if (download.IsDownloaded)
//         {
//           response = new JsonPacket("AddTrack", new
//           {
//             Success = true,
//             Message = $"Track '{author} - {title}' downloaded successfully"
//           });
//           _session.Broadcast(response.ToJson());

//           Logger.Log(LogLevel.Info, $"Downloaded '{author} - {title}' successfully");
//           Core.Instance.Player.Playlist.AddTrack(download);
//         }
//       }
//       else
//       {
//         var check1 = Core.Instance.Player.Playlist.GetTrack(title, author);
//         if (check1 is not null)
//         {
//           var response = new JsonPacket("AddTrack", new
//           {
//             Success = false,
//             Message = $"Track '{author} - {title}' is already in playlist"
//           });
//           _session.Broadcast(response.ToJson());
//           return;
//         }
//         // if something buggy will happen then this is like kinda
//         var song = Core.Instance.MediaDownloader.GetDownloadedSong(title, author);
//         if (song is not null)
//         {
//           var response = new JsonPacket("AddTrack", new
//           {
//             Success = true,
//             Message = $"Track '{author} - {title}' added to playlist"
//           });
//           Core.Instance.Player.Playlist.AddTrack(song);
//           _session.Broadcast(response.ToJson());
//         }
//       }
//     }
//   }
// }