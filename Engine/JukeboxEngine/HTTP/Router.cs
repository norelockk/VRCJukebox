using System.Net;
using System.Reflection;
using JukeboxEngine.Interfaces;

namespace JukeboxEngine.HTTP
{
  public class Router
  {
    private readonly Dictionary<(string, string), (Action<HttpListenerRequest, HttpListenerResponse>, List<IMiddleware>)> _routes;
    private readonly IMiddlewareFactory _middlewareFactory;

    public Router(IMiddlewareFactory middlewareFactory)
    {
      _routes = new Dictionary<(string, string), (Action<HttpListenerRequest, HttpListenerResponse>, List<IMiddleware>)>();
      _middlewareFactory = middlewareFactory;
    }

    public void AddRoute(string path, string method, Action<HttpListenerRequest, HttpListenerResponse> handler, List<IMiddleware> middlewares)
    {
      _routes[(path.ToLower(), method.ToUpper())] = (handler, middlewares);
    }

    public async Task<bool> RouteRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
    {
      var routeKey = (request.Url!.LocalPath.ToLower(), request.HttpMethod.ToUpper());

      if (request.HttpMethod == "OPTIONS")
      {
        response.StatusCode = (int)HttpStatusCode.NoContent;
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        response.Close();
        return true;
      }

      if (_routes.TryGetValue(routeKey, out var route))
      {
        var (handler, middlewares) = route;

        foreach (var middleware in middlewares)
        {
          if (!await middleware.InvokeAsync(request, response) || !middleware.Invoke(request, response))
            return true;
        }

        handler(request, response);
        return true;
      }

      return false;
    }

    public void RegisterControllers(Assembly assembly)
    {
      foreach (var type in assembly.GetTypes())
      {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
          var routeAttributes = method.GetCustomAttributes<RouteAttribute>();
          var middlewareAttributes = method.GetCustomAttributes<MiddlewareAttribute>();

          foreach (var routeAttribute in routeAttributes)
          {
            var middlewares = middlewareAttributes
                .SelectMany(attr => attr.MiddlewareTypes)
                .Select(middlewareType => _middlewareFactory.CreateMiddleware(middlewareType))
                .Cast<IMiddleware>()
                .ToList();

            AddRoute(routeAttribute.Path, routeAttribute.Method,
                (Action<HttpListenerRequest, HttpListenerResponse>)Delegate.CreateDelegate(
                    typeof(Action<HttpListenerRequest, HttpListenerResponse>), method), middlewares);
          }
        }
      }
    }
  }
}
