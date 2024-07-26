using NAudio.Wave;
using JukeboxEngine.Models.Classes;
using JukeboxEngine.VRChat;
using JukeboxEngine.Utils;
using JukeboxEngine.Enums;

namespace JukeboxEngine.Audio;

public class Player : WaveOutEvent
{
  public readonly Playlist Playlist;

  public Player(Playlist playlist, VRCClient client)
  {
    Logger.Log(ELogLevel.Debug, "Player(): constructor");

    _client = client;
    Playlist = playlist;

    Initialize();
  }

  public long GetCurrentPosition()
  {
    if (PlaybackState != PlaybackState.Stopped || CurrentAudio is not null)
      return CurrentAudio!.Position / (CurrentAudio.WaveFormat.SampleRate * CurrentAudio.WaveFormat.BlockAlign);

    return 0;
  }

  public long GetCurrentLength()
  {
    if (PlaybackState != PlaybackState.Stopped || CurrentAudio is not null)
      return CurrentAudio!.Length / (CurrentAudio.WaveFormat.SampleRate * CurrentAudio.WaveFormat.BlockAlign);

    return 1;
  }

  private readonly VRCClient? _client = null;

  public void _Play()
  {
    if (PlaybackState != PlaybackState.Playing)
    {
      if (Playlist.IsEmpty())
      {
        Logger.Log(ELogLevel.Warning, "Playlist empty");
        return;
      }

      PlayTrack();
    }
  }

  public AudioFile? CurrentAudio;
  private readonly VirtualOutputDevice virtualOutputDevice = new();

  private void PlayTrack()
  {
    Track? currentTrack = Playlist.GetCurrentTrack();

    if (currentTrack is not null)
    {
      CurrentAudio = new AudioFile(currentTrack!.SavePath!);

      try
      {
        Init(CurrentAudio);
      }
      catch (Exception ex)
      {
        Logger.Log(ELogLevel.Error, $"PlayTrack error: {ex.Message}");
        throw;
      }

      Play();
      Logger.Log(ELogLevel.Info, $"Now playing: {currentTrack.Author} - {currentTrack.Name}");
    }
  }

  // fixme: proper gathering an device index for NAudio library
  // because it dosen't like the basic indexes (from 0's, i mean it liked before but now idfk what happened)  
  private void Initialize()
  {
    PlaybackStopped += OnPlaybackStopped!;

    try
    {
      DeviceNumber = virtualOutputDevice.GetIndex() + 1;
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"VirtualOutputDevice error: ${ex.Message}");
      throw;
    }

    InitializeOsc();
  }

  private InvokeRepeating? oscInterval;
  private void InitializeOsc()
  {
    oscInterval = new(UpdateOscAsync, 2000);
    oscInterval.Start();
  }

  private const int progressResolution = 13;

  private const char progressStart = '\u2523';
  private const char progressEnd = '\u252B';
  private const char progressDot = '\u25CF';
  private const char progressLine = '\u2501';

  private const string iconPlay = "\u25B6\uFE0F";
  private const string iconPause = "\u23F8\uFE0F";
  private const string iconStopped = "\u23F9\uFE0F";

  private async void UpdateOscAsync()
  {
    var icon = string.Empty;
    var message = string.Empty;

    switch (PlaybackState)
    {
      default:
        {
          icon = iconStopped;
          break;
        }
      case PlaybackState.Paused:
        {
          icon = iconPause;
          break;
        }
      case PlaybackState.Playing:
        {
          icon = iconPlay;
          break;
        }
    }

    message += GetCurrentProgress();

    Track? currentTrack = !Playlist.IsEmpty() ? Playlist.GetCurrentTrack() : null;

    message += $"{(
      CurrentAudio is not null
      ? $"\n{icon} {currentTrack!.Author} - {currentTrack!.Name}"
      : $"{icon}")}";

    message += $"\n{Constants.projectName} {Constants.projectVersion}";

    if (_client is not null)
      await _client.SendGameMessage(OscConstants.OSC_PATH_CHATBOX_INPUT, message, true);
  }

  private string GetCurrentProgress()
  {
    var progress = string.Empty;

    if (CurrentAudio is not null)
    {
      float length = GetCurrentLength()!;
      float position = GetCurrentPosition()!;

      string lengthTime = NumberUtils.ConvertSeconds(length);
      string currentTime = NumberUtils.ConvertSeconds(position);

      progress += $"[ {currentTime} / {lengthTime} ]\n";

      double index = Math.Floor((position / length) * progressResolution);

      progress += progressStart;

      for (int i = 0; i < progressResolution; i++)
        progress += i == index ? progressDot : progressLine;

      progress += progressEnd;
    }

    return progress;
  }

  private void OnPlaybackStopped(object sender, StoppedEventArgs e)
  {
    if (e.Exception != null)
      Logger.Log(ELogLevel.Error, $"playback error: {e.Exception.Message}");

    if (!Playlist.isRepeatMode)
    {
      var nextTrack = Playlist.GetNextTrack();

      if (nextTrack is not null)
      {
        Playlist.NextTrack();
        UpdateOscAsync();
      }
      else
      {
        CurrentAudio!.Position = 0;
        Playlist.currentTrackIndex = 0;
        return;
      }
    }
    else
      Playlist.NextTrack();

    PlayTrack();
  }
}