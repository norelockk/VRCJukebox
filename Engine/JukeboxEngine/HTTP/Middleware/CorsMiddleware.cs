using System.Net;
using JukeboxEngine.Interfaces;

namespace JukeboxEngine.HTTP.Middleware;

public class CorsMiddleware : IMiddleware
{
  private readonly string _allowedOrigins;
  private readonly string _allowedMethods;
  private readonly string _allowedHeaders;

  public CorsMiddleware() : this("*", "GET, POST, PUT, DELETE, OPTIONS", "Content-Type, Authorization") { }

  public CorsMiddleware(string allowedOrigins = "*", string allowedMethods = "GET, POST, PUT, DELETE, OPTIONS", string allowedHeaders = "Content-Type, Authorization")
  {
    _allowedOrigins = allowedOrigins;
    _allowedMethods = allowedMethods;
    _allowedHeaders = allowedHeaders;
  }

  public bool Invoke(HttpListenerRequest request, HttpListenerResponse? response = null)
  {
    if (response is null)
      return false;

    var headers = response.Headers;
    bool hasCorsHeaders = headers["Access-Control-Allow-Origin"] is not null;

    if (!hasCorsHeaders)
    {
      headers.Add("Access-Control-Allow-Origin", _allowedOrigins);
      headers.Add("Access-Control-Allow-Methods", _allowedMethods);
      headers.Add("Access-Control-Allow-Headers", _allowedHeaders);
    }

    return true;
  }
}
