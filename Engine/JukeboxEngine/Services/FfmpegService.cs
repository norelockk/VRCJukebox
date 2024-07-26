using FFMpegCore;
using GithubReleaseDownloader;
using GithubReleaseDownloader.Entities;
using SharpCompress.Common;
using SharpCompress.Readers;

using JukeboxEngine.Enums;

namespace JukeboxEngine.Services;

public class FfmpegService
{
  public FfmpegService(Config config)
  {
    Logger.Log(ELogLevel.Debug, "FfmpegService(): constructor");

    _config = config;
  }

  private readonly Config _config;

  public async Task<bool> StreamConvert(string url, string fileName)
  {
    return await FFMpegArguments
      .FromUrlInput(new Uri(url))
      .OutputToFile(fileName)
      .ProcessAsynchronously();
  }

  public void ConfigureFfmpeg()
  {
    GlobalFFOptions.Configure(new FFOptions
    {
      BinaryFolder = _config.FfmpegLocation,
      TemporaryFilesFolder = _config.TempFilesLocation
    });
  }

  public async Task<bool> DownloadFfmpeg()
  {
    if (new DirectoryInfo(_config.FfmpegLocation).EnumerateFiles().Count() >= 3)
      return true;

    var release = await ReleaseManager.Instance.GetLatestAsync("Tyrrrz", "FFmpegBin");
    if (release is null || !release.Assets.Any()) return false;

    var releaseAsset = new ReleaseAsset();

    string actualPlatform = OperatingSystem.IsWindows() ? "Windows"
      : OperatingSystem.IsLinux() ? "Linux"
      : OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst() ? "Mac"
      : "Unknown";

    switch (actualPlatform)
    {
      case "Mac":
        {
          releaseAsset = release.Assets.First(asset => asset.Name.EndsWith(OperatingSystem.IsMacOS() ? "osx-x64.zip" : "osx-arm64.zip"));
          break;
        }

      case "Linux":
        {
          releaseAsset = release.Assets.First(asset => asset.Name.EndsWith("linux-x64.zip"));
          break;
        }

      case "Windows":
        {
          releaseAsset = release.Assets.First(asset => asset.Name.EndsWith("windows-x64.zip"));
          break;
        }

      default: throw new Exception("Unknown distribution");
    }

    Logger.Log(ELogLevel.Info, $"Downloading Ffmpeg for {actualPlatform}, that may take a while..");
    var downloadInfo = await AssetDownloader.Instance.DownloadAssetAsync(releaseAsset, _config.FfmpegLocation);

    await Task.Run(async () =>
    {
      await using var stream = File.OpenRead(downloadInfo.Path);
      using var reader = ReaderFactory.Open(stream);

      while (reader.MoveToNextEntry())
      {
        if (!reader.Entry.IsDirectory)
        {
          reader.WriteEntryToDirectory(_config.FfmpegLocation, new ExtractionOptions()
          {
            ExtractFullPath = true,
            Overwrite = true
          });
        }
      }
    }
    ).ContinueWith(_ =>
    {
      File.Delete(downloadInfo.Path);
      Logger.Log(ELogLevel.Info, $"Downloaded Ffmpeg for {actualPlatform} successfully");
    });

    return true;
  }
}