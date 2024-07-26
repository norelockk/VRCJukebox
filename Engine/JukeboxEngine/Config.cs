using Russkyc.Configuration;

using JukeboxEngine.Enums;

namespace JukeboxEngine;

public class Config : ConfigProvider
{
  public Config() : base($"{AppContext.BaseDirectory}config.json")
  {
    Logger.Log(ELogLevel.Debug, "Config(): constructor");
    Init();
  }

  private void Init()
  {
    try
    {
      if (string.IsNullOrWhiteSpace(FfmpegLocation))
        FfmpegLocation = $"{AppContext.BaseDirectory}ffmpeg\\";
    }
    catch (Exception)
    {
      FfmpegLocation = $"{AppContext.BaseDirectory}ffmpeg\\";
      throw;
    }

    try
    {
      if (string.IsNullOrWhiteSpace(DownloadLocation))
        DownloadLocation = $"{AppContext.BaseDirectory}songs\\";
    }
    catch (Exception)
    {
      DownloadLocation = $"{AppContext.BaseDirectory}songs\\";
      throw;
    }

    try
    {
      if (string.IsNullOrWhiteSpace(TempFilesLocation))
        TempFilesLocation = $"{AppContext.BaseDirectory}temp\\";
    }
    catch (Exception)
    {
      TempFilesLocation = $"{AppContext.BaseDirectory}temp\\";
      throw;
    }
  }

  public string FfmpegLocation
  {
    get => GetValue<string>(nameof(FfmpegLocation));
    set => SetValue(nameof(FfmpegLocation), value);
  }

  public string DownloadLocation
  {
    get => GetValue<string>(nameof(DownloadLocation));
    set => SetValue(nameof(DownloadLocation), value);
  }

  public string TempFilesLocation
  {
    get => GetValue<string>(nameof(TempFilesLocation));
    set => SetValue(nameof(TempFilesLocation), value);
  }
}