using SpotifyExplode;
using YoutubeExplode;

using JukeboxEngine.HTTP;
using JukeboxEngine.Enums;
using JukeboxEngine.Audio;
using JukeboxEngine.Media;
using JukeboxEngine.Utils;
using JukeboxEngine.VRChat;
using JukeboxEngine.Services;
using JukeboxEngine.WebSocket;
using JukeboxEngine.Models.Classes;

namespace JukeboxEngine;

public class Core : Singleton<Core>
{
  public Core()
  {
    Logger.Log(ELogLevel.Debug, "Core(): constructor");
    Initialize();
  }

  public void CollectGarbage()
  {
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
  }

  public static readonly Config Config = new();
  private readonly static SpotifyClient SpotifyClient = new();
  private readonly static YoutubeClient YoutubeClient = new();

  private FileService? FileService;
  private FfmpegService? FfmpegService;
  private MetadataService? MetadataService;

  public bool Initialized { get; private set; } = false;
  public Player? Player { get; private set; }
  public Playlist? Playlist { get; private set; }
  public VRCClient? Client { get; private set; }
  public HTTPServer? HTTPServer { get; private set; }
  public MediaSearch? MediaSearch { get; private set; }
  public MediaDownloader? MediaDownloader { get; private set; }
  public WebSocketService? WebSocketService { get; private set; }

  private async void AsyncInit()
  {
    if (Initialized)
      return;

    FileService = new(Config);
    FfmpegService = new(Config);

    await FfmpegService.DownloadFfmpeg();

    MetadataService = new(SpotifyClient, YoutubeClient);
    WebSocketService = new();

    MediaSearch = new();
    MediaDownloader = new(Config, FileService, FfmpegService, MetadataService);

    Client = new(Config);
    Playlist = new(Config);
    Player = new(Playlist, Client);
    HTTPServer = new();

    Initialized = true;
  }

  private void Initialize()
  {
    AsyncInit();

    Logger.Log(ELogLevel.Info, "Core(): initialize success");
  }
}