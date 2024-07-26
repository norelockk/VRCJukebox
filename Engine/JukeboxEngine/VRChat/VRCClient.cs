using System.Net;
using OscQueryLibrary;
using LucHeart.CoreOSC;
using System.Text.Json;
using OscQueryLibrary.Utils;
using JukeboxEngine.Enums;

namespace JukeboxEngine.VRChat;

public class VRCClient
{
  private OscDuplex? _gameConnection = null;
  private List<OscQueryServer> _oscQueryServers = new();
  private OscQueryServer? _currentOscQueryServer = null;
  private CancellationTokenSource _loopCancellationToken = new();
  private readonly HashSet<string> _parameterList = new()
    {
        "AFK",
        "MuteSelf"
    };

  private Task FindVrcClient(OscQueryServer oscQueryServer, IPEndPoint ipEndPoint)
  {
    _loopCancellationToken.Cancel();
    _loopCancellationToken = new CancellationTokenSource();
    _gameConnection?.Dispose();
    _gameConnection = null;

    Logger.Log(ELogLevel.Debug, $"Connected to VRC Client at {ipEndPoint}");
    // Logger.Log(ELogLevel.Debug, $"Starting listening for VRC Client at {oscQueryServer.OscReceivePort}");

    _gameConnection = new OscDuplex(new IPEndPoint(ipEndPoint.Address, oscQueryServer.OscReceivePort), ipEndPoint);
    _currentOscQueryServer = oscQueryServer;

    ErrorHandledTask.Run(ReceiverLoopAsync);
    return Task.CompletedTask;
  }

  private async Task ReceiverLoopAsync()
  {
    var currentCancellationToken = _loopCancellationToken.Token;

    while (!currentCancellationToken.IsCancellationRequested)
    {
      try
      {
        await ReceiveLogic();
      }
      catch (Exception e)
      {
        Logger.Log(ELogLevel.Error, $"Loop receiver error: {e.Message}");
      }
    }
  }

  private async Task ReceiveLogic()
  {
    if (_gameConnection is null) return;

    OscMessage received;

    try
    {
      received = await _gameConnection.ReceiveMessageAsync();
    }
    catch (Exception e)
    {
      Logger.Log(ELogLevel.Error, $"Error receiving message: {e.Message}");
      return;
    }

    var addr = received.Address;

    switch (addr)
    {
      // default:
      //   {
      //     Console.WriteLine($"Parm: {received.Address}/{JsonConvert.SerializeObject(received.Arguments).ToString()}");
      //     break;
      //   }

      case "/avatar/change":
        {
          var avatarId = received.Arguments.ElementAtOrDefault(0);
          Console.WriteLine($"Avatar changed: {avatarId}");
          await _currentOscQueryServer!.GetParameters();
          break;
        }

      case "/avatar/parameters/MuteSelf":
        {
          var isMuted = received.Arguments.ElementAtOrDefault(0);
          if (isMuted != null)
            IsMuted = (bool)isMuted;
          Console.WriteLine($"muted: {IsMuted}");
          break;
        }
    }
  }

  private Task UpdateAvailableParameters(Dictionary<string, object?> parameterList, string s)
  {
    AvailableParameters.Clear();
    foreach (var parameter in parameterList)
    {
      var parameterName = parameter.Key.Replace("/avatar/parameters/", "");
      if (_parameterList.Contains(parameterName))
        AvailableParameters.Add(parameterName);

      if (parameterName == "MuteSelf" && parameter.Value is not null)
        IsMuted = ((JsonElement)parameter.Value).GetBoolean();
    }

    return Task.CompletedTask;
  }

  public VRCClient(Config config)
  {
    var server = new OscQueryServer(Constants.projectName, IPAddress.Loopback);
    server.FoundVrcClient += FindVrcClient;
    server.ParameterUpdate += UpdateAvailableParameters;

    _oscQueryServers.Add(server);
    server.Start();
  }

  public bool IsMuted;
  public readonly HashSet<string> AvailableParameters = new();

  public async Task SendGameMessage(string address, params object?[]? arguments)
  {
    if (_gameConnection is null) return;
    arguments ??= Array.Empty<object>();

    await _gameConnection.SendAsync(new OscMessage(address, arguments));
  }
}