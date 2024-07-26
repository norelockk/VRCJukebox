using System.Net;
using JukeboxEngine.Enums;
using JukeboxEngine.Interfaces;

namespace JukeboxEngine.HTTP.Middleware;

public class LoggingMiddleware : MiddlewareBase
{
  public LoggingMiddleware() : base() {}

  public async Task<bool> Invoke(HttpListenerContext context)
  {
    Logger.Log(ELogLevel.Info, $"{context.Request.HttpMethod} {context.Request.Url}");
    return await Task.FromResult(true);
  }
}