using System.Reflection;

namespace JukeboxEngine.HTTP;

public static class RouterExtensions
{
  public static void RegisterApiControllers(this Router router, Assembly assembly)
  {
    router.RegisterControllers(assembly);
  }
}