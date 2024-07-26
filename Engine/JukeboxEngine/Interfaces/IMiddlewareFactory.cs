using JukeboxEngine.Interfaces;

public interface IMiddlewareFactory
{
    IMiddleware CreateMiddleware(Type middlewareType, params object[] parameters);
}