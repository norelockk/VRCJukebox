namespace JukeboxEngine.Utils;

public class Singleton<T> where T : new()
{
  private readonly static Lazy<T> instance = new Lazy<T>(() => new T());

  public static T Instance => instance.Value;

  protected Singleton() { }
}