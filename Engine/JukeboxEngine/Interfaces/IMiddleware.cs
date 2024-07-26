using System.Net;

namespace JukeboxEngine.Interfaces;

public interface IMiddleware
{
  Task<bool> InvokeAsync(HttpListenerRequest request, HttpListenerResponse? response = null) =>
      Task.FromResult(true);

  bool Invoke(HttpListenerRequest request, HttpListenerResponse? response = null) =>
      true;
}