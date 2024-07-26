using System.Net;
using System.Reflection;
using System.Text;
using JukeboxEngine.Interfaces;
using Newtonsoft.Json;

namespace JukeboxEngine.HTTP;

public class RequestHandler
{
  private readonly string _publicDirectory;
  private readonly Router _router;

  public RequestHandler(string publicDirectory, Router router)
  {
    _router = router;
    _publicDirectory = publicDirectory;
  }

  public async Task ProcessRequestAsync(HttpListenerContext context)
  {
    var build = string.Empty;
#if DEBUG
    build = "development";
#endif

#if RELEASE
        build = "release";
#endif

    context.Response.Headers["Server"] = null;
    context.Response.Headers.Add("X-Norelock-Project", $"{Constants.projectName}/{Constants.projectVersion}-{build}");
    context.Response.Headers.Add("Expires", "0");
    context.Response.Headers.Add("Pragma", "no-cache");
    context.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
    context.Response.Headers.Add("Surrogate-Control", "no-store");

    if (await _router.RouteRequestAsync(context.Request, context.Response))
      return;

    await ServeStaticFilesAsync(context);
  }

  private async Task ServeStaticFilesAsync(HttpListenerContext context)
  {
    var request = context.Request;
    var response = context.Response;
    var requestedPath = request.Url!.LocalPath.TrimStart('/');

    if (string.IsNullOrEmpty(requestedPath))
      requestedPath = "index.html";

    var requestedFile = Path.Combine(_publicDirectory, requestedPath);

    if (File.Exists(requestedFile))
    {
      try
      {
        byte[] buffer = await File.ReadAllBytesAsync(requestedFile);
        response.ContentType = GetContentType(Path.GetExtension(requestedFile));
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
      }
      catch (Exception ex)
      {
        SendPlain(response, $"Error: {ex.Message}", HttpStatusCode.InternalServerError);
      }
    }
    else
    {
      SendJson(response, new { code = "NOT_FOUND" }, HttpStatusCode.NotFound);
    }

    response.OutputStream.Close();
  }

  public string GetContentType(string extension)
  {
    return extension.ToLower() switch
    {
      ".html" => "text/html",
      ".css" => "text/css",
      ".js" => "application/javascript",
      ".json" => "application/json",
      ".png" => "image/png",
      ".jpg" => "image/jpeg",
      ".gif" => "image/gif",
      _ => "application/octet-stream",
    };
  }

  public async Task<bool> InvokeMiddlewares(HttpListenerRequest request, HttpListenerResponse response, MethodInfo methodInfo, List<IMiddleware> middlewares)
  {
    foreach (var middleware in middlewares)
    {
      if (!await middleware.InvokeAsync(request, response) || !middleware.Invoke(request, response))
        return false;
    }
    return true;
  }

  public void SendJson(HttpListenerResponse response, object data, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    var json = JsonConvert.SerializeObject(data);
    var buffer = Encoding.UTF8.GetBytes(json);

    response.ContentType = "application/json";
    response.ContentLength64 = buffer.Length;
    response.StatusCode = (int)statusCode;

    using (var output = response.OutputStream)
    {
      output.Write(buffer, 0, buffer.Length);
    }
  }

  public void SendPlain(HttpListenerResponse response, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    var buffer = Encoding.UTF8.GetBytes(message);

    response.StatusCode = (int)statusCode;
    response.ContentType = "text/plain";
    response.ContentLength64 = buffer.Length;

    using (var output = response.OutputStream)
    {
      output.Write(buffer, 0, buffer.Length);
    }
  }

  public void SendFile(HttpListenerResponse response, byte[] fileContent, string contentType, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    response.ContentType = contentType;
    response.ContentLength64 = fileContent.Length;
    response.StatusCode = (int)statusCode;

    using (var output = response.OutputStream)
    {
      output.Write(fileContent, 0, fileContent.Length);
    }
  }
}
