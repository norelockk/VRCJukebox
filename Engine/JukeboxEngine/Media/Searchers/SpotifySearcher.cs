using JukeboxEngine.Enums;
using JukeboxEngine.Utils;
using SpotifyExplode;
using SpotifyExplode.Search;

namespace JukeboxEngine.Media.Searchers;

public class SpotifySearcher
{
  private readonly SpotifyClient spotifyClient = new SpotifyClient();

  public async Task<IEnumerable<ISearchResult>> SearchAsync(string query, int limit = 20, int skip = 0)
  {
    if (limit > 50)
    {
      Logger.Log(ELogLevel.Warning, "Search limit exceeded");
      return Enumerable.Empty<ISearchResult>();
    }

    try
    {
      var cleanQuery = query.ToCleanQueryString();

      if (query.Contains("album", StringComparison.InvariantCultureIgnoreCase))
      {
        return await spotifyClient.Search.GetAlbumsAsync(cleanQuery, skip, limit);
      }

      if (query.Contains("playlist", StringComparison.InvariantCultureIgnoreCase))
      {
        return await spotifyClient.Search.GetPlaylistsAsync(cleanQuery, skip, limit);
      }

      return await spotifyClient.Search.GetTracksAsync(cleanQuery, skip, limit);
    }
    catch (Exception e)
    {
      Logger.Log(ELogLevel.Error, $"SearchAsync error: {e.Message}");
      return Enumerable.Empty<ISearchResult>();
    }
  }

}