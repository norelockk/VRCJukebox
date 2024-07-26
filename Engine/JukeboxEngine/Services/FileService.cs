using JukeboxEngine.Enums;
using JukeboxEngine.Interfaces;

namespace JukeboxEngine.Services;

public class FileService : IFileService
{
  public FileService(Config config)
  {
    Logger.Log(ELogLevel.Debug, "FileService(): constructor");

    _config = config;
    SetupDirectories();
  }

  private readonly Config _config;

  public void SetupDirectories()
  {
    if (string.IsNullOrWhiteSpace(_config.DownloadLocation)) _config.DownloadLocation = Environment.CurrentDirectory + "\\songs\\";
    if (!Directory.Exists(_config.DownloadLocation)) Directory.CreateDirectory(_config.DownloadLocation);

    if (string.IsNullOrWhiteSpace(_config.TempFilesLocation)) _config.TempFilesLocation = Environment.CurrentDirectory + "\\temp\\";
    if (!Directory.Exists(_config.TempFilesLocation)) Directory.CreateDirectory(_config.TempFilesLocation);

    if (string.IsNullOrWhiteSpace(_config.FfmpegLocation)) _config.FfmpegLocation = Environment.CurrentDirectory + "\\ffmpeg\\";
    if (!Directory.Exists(_config.FfmpegLocation)) Directory.CreateDirectory(_config.FfmpegLocation);
  }

  public bool CreateMediaDirectory(string basePath, string directoryName)
  {
    var mediaDirectory = $"{basePath}{directoryName}\\";

    if (Directory.Exists(mediaDirectory))
      return true;

    Directory.CreateDirectory(mediaDirectory);
    return Directory.Exists(mediaDirectory);
  }
}