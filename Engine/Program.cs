namespace JukeboxEngine;

internal class Program
{
  private static Core core;

  static void Main(string[] args)
  {
    Console.WriteLine($"{Constants.projectLogo}\n v{Constants.projectVersion}\n");

    core = Core.Instance;

    while (true)
    {
      core.CollectGarbage();
      Thread.Sleep(10 * 1000);
    }
  }
}