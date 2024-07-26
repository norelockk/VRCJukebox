namespace JukeboxEngine.HTTP;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RouteAttribute : Attribute
{
  public string? Path { get; }
  public string? Method { get; }

  public RouteAttribute(string path, string method = "GET")
  {
    Path = path;
    Method = method.ToUpper();
  }
}
