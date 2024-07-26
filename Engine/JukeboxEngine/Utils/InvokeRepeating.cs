namespace JukeboxEngine.Utils;

public class InvokeRepeating
{
  private Timer? timer;
  private Action? repeatedAction;
  private int interval;

  public InvokeRepeating(Action action, int interval)
  {
    this.interval = interval;
    repeatedAction = action;
  }

  public void Start()
  {
    timer = new Timer(InvokeAction!, null, 0, interval);
  }

  public void Stop()
  {
    timer?.Change(Timeout.Infinite, Timeout.Infinite);
  }

  private void InvokeAction(object state)
  {
    repeatedAction?.Invoke();
  }
}