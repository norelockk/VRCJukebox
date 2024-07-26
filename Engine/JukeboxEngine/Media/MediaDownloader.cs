using JsonFlatFileDataStore;
using JukeboxEngine.Enums;
using JukeboxEngine.Utils;
using JukeboxEngine.Services;
using JukeboxEngine.Models.Classes;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JukeboxEngine.Media;

public class MediaDownloader
{
  public MediaDownloader(Config config, FileService fileService, FfmpegService ffmpegService, MetadataService metadataService)
  {
    Logger.Log(ELogLevel.Debug, "MediaDownloader(): constructor");

    _config = config;
    _fileService = fileService;
    _ffmpegService = ffmpegService;
    _metadataService = metadataService;

    _mediaDataStore = new DataStore($"{_config.DownloadLocation}downloads.json", reloadBeforeGetCollection: true);
  }

  private readonly Config _config;
  private readonly DataStore _mediaDataStore;
  private readonly FileService _fileService;
  private readonly FfmpegService _ffmpegService;
  private readonly MetadataService _metadataService;

  public async Task<Track> Download(string author, string title, string url, string? collection = null)
  {
    var trackDownload = new Track
    {
      Name = title,
      Author = author
    };

    var songSaveName = $"{author.ToPathSafeString()} - {title.ToPathSafeString()}.mp3";
    var artworkPath = string.Empty;

    try
    {
      await _ffmpegService.DownloadFfmpeg();
      _ffmpegService.ConfigureFfmpeg();

      var streamUrl = await _metadataService.GetYoutubeSongStream(url);

      if (string.IsNullOrWhiteSpace(streamUrl))
        return trackDownload;

      // Fetch and save artwork
      var artworkUrl = await _metadataService.GetArtwork(url, $"{_config.DownloadLocation}{title.ToPathSafeString()}_artwork.jpg");
      if (artworkUrl != null)
      {
        artworkPath = $"{_config.DownloadLocation}{title.ToPathSafeString()}_artwork.jpg";
      }

      if (collection is not null)
      {
        var songAlbumPath = $"{_config.DownloadLocation}{collection}\\";

        trackDownload.Collection = collection;
        trackDownload.SavePath = $"{songAlbumPath}{songSaveName}";
        trackDownload.ArtworkPath = artworkPath; // Set the artwork path
        trackDownload.IsDownloaded = await _ffmpegService.StreamConvert(streamUrl, $"{songAlbumPath}{songSaveName}");

        if (trackDownload.IsDownloaded)
          await _mediaDataStore.GetCollection<Track>().InsertOneAsync(trackDownload);

        return trackDownload;
      }

      trackDownload.SavePath = $"{_config.DownloadLocation}{songSaveName}";
      trackDownload.ArtworkPath = artworkPath; // Set the artwork path
      trackDownload.IsDownloaded = await _ffmpegService.StreamConvert(streamUrl, $"{_config.DownloadLocation}{songSaveName}");

      if (trackDownload.IsDownloaded)
        await _mediaDataStore.GetCollection<Track>().InsertOneAsync(trackDownload);

      return trackDownload;
    }
    catch (Exception)
    {
      if (File.Exists(trackDownload.SavePath))
        File.Delete(trackDownload.SavePath);

      if (File.Exists(artworkPath))
        File.Delete(artworkPath);

      return trackDownload;
    }
  }

  public async IAsyncEnumerable<Track> DownloadAlbum(string title, string id)
  {
    _fileService.CreateMediaDirectory(_config.DownloadLocation, title.ToPathSafeString());

    foreach (var track in await _metadataService.GetAlbumTracksMetadata(id))
    {
      var authors = track.Artists.Select(artist => artist.Name).Take(3).ToDelimitedString();

      yield return await Download(authors, track.Title, track.Url, title.ToPathSafeString());
    }
  }

  public async IAsyncEnumerable<Track> DownloadPlaylist(string title, string id)
  {
    _fileService.CreateMediaDirectory(_config.DownloadLocation, title.ToPathSafeString());

    foreach (var track in await _metadataService.GetPlaylistTracksMetadata(id))
    {
      var authors = track.Artists.Select(artist => artist.Name).Take(3).ToDelimitedString();
      
      yield return await Download(authors, track.Title, track.Url, title.ToPathSafeString());
    }
  }

  public Track? GetDownloadedSong(string title, string author)
  {
    return _mediaDataStore.GetCollection<Track>()
      .AsQueryable()
      .FirstOrDefault(song => song.Name == title && song.Author == author);
  }

  public bool GetIfDownloadExists(string title, string author)
  {
    return _mediaDataStore.GetCollection<Track>().AsQueryable().Any(track => track.Name!.Equals(title) && track.Author!.Equals(author));
  }

  public bool GetIfDownloadCollectionExists(string collection)
  {
    return _mediaDataStore.GetCollection<Track>().AsQueryable().Any(track => track.Collection == collection);
  }
}
