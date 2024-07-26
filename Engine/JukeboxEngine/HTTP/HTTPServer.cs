using System.Net;
using System.Diagnostics;
using JukeboxEngine.Enums;
using JukeboxEngine.HTTP.API;

namespace JukeboxEngine.HTTP;

public class HTTPServer
{
  public HTTPServer()
  {
    Logger.Log(ELogLevel.Debug, "HTTPServer(): constructor");

    _publicDirectory = Path.Combine(Directory.GetCurrentDirectory(), "HTML");
    Logger.Log(ELogLevel.Debug, $"public directory: {_publicDirectory}");

    _listener = new HttpListener();
    _middlewareFactory = new MiddlewareFactory(); // Create MiddlewareFactory
    _router = new Router(_middlewareFactory); // Pass it to Router
    _requestHandler = new RequestHandler(_publicDirectory, _router);

    AsyncInit();
  }

  private static void AddUrlAcl(string url)
  {
    // if (!IsAdministrator()) {
    // return;
    // }

    var command = $"http add urlacl url={url} user=Everyone";
    var process = new Process
    {
      StartInfo = new ProcessStartInfo
      {
        FileName = "netsh",
        Arguments = command,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      }
    };

    process.Start();
    process.WaitForExit();

    if (process.ExitCode != 0)
    {
      Console.WriteLine("Access is denied to open HTTP server. Please run the following command as an administrator to grant access:");
      Console.WriteLine($"netsh http add urlacl url={url} user=Everyone");

      throw new Exception("Failed to add URL ACL");
    }
  }

  private readonly RequestHandler _requestHandler;
  private readonly HttpListener _listener;
  private readonly Router _router;
  private readonly IMiddlewareFactory _middlewareFactory;

  private async void AsyncInit()
  {
    MapRoutes(router =>
    {
      RequestSearch.Initialize(_requestHandler);
      router.RegisterApiControllers(typeof(RequestSearch).Assembly);

      RequestBase.Initialize(_requestHandler);
      router.RegisterApiControllers(typeof(RequestBase).Assembly);
    });

    await StartAsync("http://+:5000/");
  }

  private readonly string _publicDirectory;

  public async Task StartAsync(string url)
  {
    try
    {
      // AddUrlAcl(url);

      _listener.Prefixes.Add(url);
      _listener.Start();

      Logger.Log(ELogLevel.Debug, $"Listening at {url}");

      while (true)
      {
        var context = await _listener.GetContextAsync();
        await HandleRequestAsync(context);
      }
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"Error in StartAsync: {ex.Message}");
    }
  }

  private async Task HandleRequestAsync(HttpListenerContext context)
  {
    await _requestHandler.ProcessRequestAsync(context);
  }

  public void MapRoutes(Action<Router> configureRoutes)
  {
    configureRoutes(_router);
  }
}
