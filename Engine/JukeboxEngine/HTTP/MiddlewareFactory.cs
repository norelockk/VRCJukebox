using JukeboxEngine.Interfaces;

namespace JukeboxEngine.HTTP;

public class MiddlewareFactory : IMiddlewareFactory
{
  // public IMiddleware CreateMiddleware(Type middlewareType, params object[] parameters)
  // {
  //   // Assume parameterless constructor; handle as necessary if parameters are required.
  //   return (IMiddleware)Activator.CreateInstance(middlewareType, parameters);
  // }

  IMiddleware IMiddlewareFactory.CreateMiddleware(Type middlewareType, params object[] parameters)
  {
    return (IMiddleware)Activator.CreateInstance(middlewareType, parameters);
  }
}
