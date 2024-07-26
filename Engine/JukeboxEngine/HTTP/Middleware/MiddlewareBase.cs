using System.Net;

namespace JukeboxEngine.Interfaces;

public abstract class MiddlewareBase : IMiddleware
{
  public virtual Task<bool> InvokeAsync(HttpListenerRequest request, HttpListenerResponse? response)
  {
    return Task.FromResult(true);
  }

  public virtual bool Invoke(HttpListenerRequest request, HttpListenerResponse? response)
  {
    return true;
  }
}
