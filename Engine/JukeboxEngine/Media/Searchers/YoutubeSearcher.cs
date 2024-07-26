using JukeboxEngine.Enums;
using JukeboxEngine.Utils;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;

namespace JukeboxEngine.Media.Searchers;

public class YoutubeSearcher
{
  private readonly YoutubeClient youtubeClient = new YoutubeClient();

  public async Task<IEnumerable<ISearchResult>> SearchAsync(string query, int count = 20)
  {
    if (count > 50)
    {
      Logger.Log(ELogLevel.Warning, "Search limit exceeded");
      return Enumerable.Empty<ISearchResult>();
    }

    try
    {
      string cleanQuery = query.ToCleanQueryString();
      return await youtubeClient.Search.GetVideosAsync(cleanQuery).CollectAsync(count);
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"SearchAsync error: {ex.Message}");
      return Enumerable.Empty<ISearchResult>();
    }
  }
}