using System.Net;
using System.Web;
using JukeboxEngine.HTTP.Middleware;

namespace JukeboxEngine.HTTP.API;

public static class RequestSearch
{
  private static RequestHandler? _requestHandler;

  public static void Initialize(RequestHandler requestHandler)
  {
    _requestHandler = requestHandler;
  }

  [Route("/api/search", "GET")]
  [Middleware(typeof(CorsMiddleware))]
  public static async void GetWSSUrl(HttpListenerRequest request, HttpListenerResponse response)
  {
    try
    {
      var queryParameters = HttpUtility.ParseQueryString(request.Url!.Query);
      if (queryParameters is null)
      {
        _requestHandler?.SendPlain(response, "params missing", HttpStatusCode.BadRequest);
        return;
      }

      var query = queryParameters["query"];

      if (string.IsNullOrEmpty(query))
      {
        _requestHandler?.SendPlain(response, "query missing", HttpStatusCode.BadRequest);
        return;
      }

      var platform = queryParameters["platform"];
      if (string.IsNullOrEmpty(platform))
      {
        _requestHandler?.SendPlain(response, "platform missing", HttpStatusCode.BadRequest);
        return;
      }

      dynamic? search = null;

      switch (platform)
      {
        case "yt":
        case "youtube":
          {
            search = await Core.Instance.MediaSearch.SearchYoutube(query);
            break;
          }
        case "spt":
        case "spotify":
          {
            search = await Core.Instance.MediaSearch.SearchSpotify(query);
            break;
          }
        default:
          {
            _requestHandler?.SendPlain(response, "platform undefined", HttpStatusCode.BadRequest);
            return;
          }
      }

      var _response = new {
        query = query,
        platform = platform,
        results = search
      };
      _requestHandler?.SendJson(response, _response, HttpStatusCode.OK);
    }
    catch (Exception ex)
    {
      _requestHandler?.SendPlain(response, $"Error: {ex.Message}", HttpStatusCode.InternalServerError);
    }
  }
}