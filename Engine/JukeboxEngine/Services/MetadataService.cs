using SpotifyExplode;
using SpotifyExplode.Tracks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using JukeboxEngine.Enums;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace JukeboxEngine.Services;

public class MetadataService
{
  public MetadataService(SpotifyClient spotifyClient, YoutubeClient youtubeClient)
  {
    Logger.Log(ELogLevel.Debug, "MetadataService(): constructor");

    _spotifyClient = spotifyClient;
    _youtubeClient = youtubeClient;
  }

  private readonly SpotifyClient _spotifyClient;
  private readonly YoutubeClient _youtubeClient;

  public async Task<int> GetAlbumTrackCount(string id)
  {
    var album = await _spotifyClient.Albums.GetAsync(id);
    return album.Tracks.Count;
  }

  public async Task<int> GetPlaylistTrackCount(string id)
  {
    var playlists = await _spotifyClient.Playlists.GetAsync(id);
    return playlists.Tracks.Count;
  }

  private string ExtractVideoId(string url)
  {
    string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&]+)";
    Match match = Regex.Match(url, pattern);

    if (match.Success)
    {
      return match.Groups[1].Value;
    }
    else
    {
      return string.Empty;
    }
  }

  public async Task<string?> GetYoutubeSongStream(string url)
  {
    try
    {
      dynamic? youtubeId = url.Contains("youtube.com") || url.Contains("youtu.be")
        ? ExtractVideoId(url)
        : await _spotifyClient.Tracks.GetYoutubeIdAsync(url);

      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync($"https://www.youtube.com/watch?v={youtubeId}");
      return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate().Url;
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }

    return null;
  }

  public async Task<string?> GetArtwork(string url, string localPath)
  {
    try
    {
      string? artworkUrl = null;

      if (url.Contains("spotify.com"))
      {
        string id = ExtractSpotifyId(url);
        var track = await _spotifyClient.Tracks.GetAsync(id);
        artworkUrl = track.Album.Images
          .OrderByDescending(img => img.Width) // Order images by width to get the largest one
          .FirstOrDefault()?.Url;
      }
      else if (url.Contains("youtube.com") || url.Contains("youtu.be"))
      {
        string videoId = ExtractVideoId(url);
        var video = await _youtubeClient.Videos.GetAsync(videoId);
        artworkUrl = video.Thumbnails
          .Where(t => t.Url.Contains("maxresdefault"))
          .OrderByDescending(t => t.Resolution.Width) // Order by width if there are multiple maxresdefault
          .FirstOrDefault()?.Url
          ?? video.Thumbnails
          .OrderByDescending(t => t.Resolution.Width) // Fallback to the highest resolution if maxresdefault not available
          .FirstOrDefault()?.Url;
      }

      if (artworkUrl != null)
      {
        using HttpClient client = new HttpClient();
        var response = await client.GetAsync(artworkUrl);
        response.EnsureSuccessStatusCode();

        await using var fs = new FileStream(localPath, FileMode.Create);
        await response.Content.CopyToAsync(fs);

        return localPath;
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }

    return null;
  }

  private string ExtractSpotifyId(string url)
  {
    string pattern = @"(?:https?:\/\/)?(?:open\.spotify\.com\/track\/|spotify:track:)([^?]+)";
    Match match = Regex.Match(url, pattern);

    if (match.Success)
    {
      return match.Groups[1].Value;
    }
    else
    {
      return string.Empty;
    }
  }

  public async Task<IEnumerable<Track>> GetAlbumTracksMetadata(string id)
  {
    var tracks = await _spotifyClient.Albums.GetAllTracksAsync(id);
    return tracks.AsEnumerable();
  }

  public async Task<IEnumerable<Track>> GetPlaylistTracksMetadata(string id)
  {
    var tracks = await _spotifyClient.Playlists.GetAllTracksAsync(id);
    return tracks.AsEnumerable();
  }
}
