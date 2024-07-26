using JukeboxEngine.Media.Searchers;

namespace JukeboxEngine.Media;

public class MediaSearch
{
  private readonly SpotifySearcher spotify = new();
  private readonly YoutubeSearcher youtube = new();

  public async Task<IEnumerable<YoutubeExplode.Search.ISearchResult>> SearchYoutube(string query)
  {
    return await youtube.SearchAsync(query);
  }

  public async Task<IEnumerable<SpotifyExplode.Search.ISearchResult>> SearchSpotify(string query)
  {
    return await spotify.SearchAsync(query);
  }
}