namespace JukeboxEngine.HTTP;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MiddlewareAttribute : Attribute
{
  public Type[] MiddlewareTypes { get; }

  public MiddlewareAttribute(params Type[] middlewareTypes)
  {
    MiddlewareTypes = middlewareTypes;
  }
}

