using System.Net;
using System.Web;
using JukeboxEngine.HTTP.Middleware;
using JukeboxEngine.Models.Classes;

namespace JukeboxEngine.HTTP.API;

public static class RequestBase
{
  private static RequestHandler? _requestHandler;

  public static void Initialize(RequestHandler requestHandler)
  {
    _requestHandler = requestHandler;
  }

  [Route("/api/playlist", "GET")]
  [Middleware(typeof(CorsMiddleware))]
  public static void GetPlaylist(HttpListenerRequest request, HttpListenerResponse response)
  {
    try
    {
      var tracks = Core.Instance.Player!.Playlist.tracks;
      var filteredTracks = tracks.Select(track => new TrackDto
      {
        Id = track.Id,
        Title = track.Name,
        Artist = track.Author,
        IsDownloaded = track.IsDownloaded,
      }).ToList();

      _requestHandler?.SendJson(response, filteredTracks, HttpStatusCode.OK);
    }
    catch (Exception ex)
    {
      _requestHandler?.SendJson(response, new { error = ex.Message }, HttpStatusCode.InternalServerError);
    }
  }

  [Route("/api/resolve-wss", "GET")]
  [Middleware(typeof(CorsMiddleware))]
  public static void GetWSSUrl(HttpListenerRequest request, HttpListenerResponse response)
  {
    try
    {
      var data = new { port = Core.Instance.WebSocketService!.Port, url = Core.Instance.WebSocketService.Location.Replace("ws://", "") };
      _requestHandler?.SendJson(response, data, HttpStatusCode.OK);
    }
    catch (Exception ex)
    {
      _requestHandler?.SendJson(response, new { error = ex.Message }, HttpStatusCode.InternalServerError);
    }
  }

  [Route("/api/artwork", "GET")]
  [Middleware(typeof(CorsMiddleware))]
  public static void GetArtwork(HttpListenerRequest request, HttpListenerResponse response)
  {
    try
    {
      var queryParameters = HttpUtility.ParseQueryString(request.Url!.Query);
      if (queryParameters is null)
      {
        _requestHandler?.SendPlain(response, "params missing", HttpStatusCode.BadRequest);
        return;
      }

      var trackId = queryParameters["trackId"];
      if (string.IsNullOrEmpty(trackId))
      {
        _requestHandler?.SendPlain(response, "trackId missing", HttpStatusCode.BadRequest);
        return;
      }

      var tracks = Core.Instance.Player!.Playlist.tracks;
      var track = tracks.Find(t => t.Id == trackId);

      if (track is null)
      {
        _requestHandler?.SendPlain(response, "track not found", HttpStatusCode.NotFound);
        return;
      }

      var artworkPath = track.ArtworkPath;
      if (string.IsNullOrEmpty(artworkPath) || !File.Exists(artworkPath))
      {
        _requestHandler?.SendPlain(response, "artwork not found", HttpStatusCode.NotFound);
        return;
      }

      var contentType = _requestHandler?.GetContentType(Path.GetExtension(artworkPath));
      var fileContent = File.ReadAllBytes(artworkPath);

      _requestHandler?.SendFile(response, fileContent, contentType!, HttpStatusCode.OK);
    }
    catch (Exception ex)
    {
      _requestHandler?.SendJson(response, new { error = ex.Message }, HttpStatusCode.InternalServerError);
    }
  }
}