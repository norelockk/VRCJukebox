using System.Net;

namespace JukeboxEngine.HTTP;

public class HttpListenerContextWrapper
{
  public HttpListenerRequest Request { get; }
  public HttpListenerResponse Response { get; }

  public HttpListenerContextWrapper(HttpListenerRequest request, HttpListenerResponse response)
  {
    Request = request;
    Response = response;
  }
}