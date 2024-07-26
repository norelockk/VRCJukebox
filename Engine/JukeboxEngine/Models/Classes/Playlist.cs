using JsonFlatFileDataStore;
using JukeboxEngine.Enums;

namespace JukeboxEngine.Models.Classes;

public class Playlist
{
  private readonly DataStore _mediaDataStore;

  public Playlist(Config config)
  {
    Logger.Log(ELogLevel.Debug, "Playlist(): constructor");

    _mediaDataStore = new DataStore($"{config.DownloadLocation}downloads.json", reloadBeforeGetCollection: true);
    LoadPlaylist();
  }

  private void LoadPlaylist()
  {
    var collection = _mediaDataStore.GetCollection<Track>().AsQueryable().Where(t => t.IsDownloaded);
    var length = collection.Count();

    try
    {
      if (collection is not null)
        foreach (var track in collection)
          AddTrack(track);
    }
    catch (Exception ex)
    {
      Logger.Log(ELogLevel.Error, $"Error while loading playlist: {ex.Message}");
    }

    Logger.Log(ELogLevel.Info, $"Loaded {length} songs");
  }

  public bool isRepeatMode = false;
  public bool isShuffleMode = false;
  public List<Track> tracks = new();

  private Random random = new();
  public int currentTrackIndex = 0;

  public void AddTrack(Track song)
  {
    tracks.Add(song);

    Logger.Log(ELogLevel.Info, $"Track '{song.Author} - {song.Name}' has been added to playlist");
  }

  public Track? GetTrack(string title, string artist)
  {
    foreach (var track in tracks)
      if (title == track.Name && artist == track.Author)
        return track;

    return null;
  }

  public void RemoveTrack(int index)
  {
    if (index >= 0 && index < tracks.Count)
    {
      var track = tracks[index];

      tracks.RemoveAt(index);
      if (index < currentTrackIndex)
        currentTrackIndex--;

      Logger.Log(ELogLevel.Info, $"Track '{track.Author} - {track.Name}' has been removed from playlist");
    }
  }

  public void RemoveTrack(Predicate<Track> match)
  {
    int index = tracks.FindIndex(match);
    if (index is not -1)
    {
      var track = tracks[index];

      tracks.RemoveAt(index);
      if (index < currentTrackIndex)
        currentTrackIndex--;

      Logger.Log(ELogLevel.Info, $"Track '{track.Author} - {track.Name}' has been removed from playlist");
    }
  }

  private void Shuffle()
  {
    if (tracks.Count <= 1)
      return;

    tracks = tracks.OrderBy(x => random.Next()).ToList();
    currentTrackIndex = 0;
  }

  public void ToggleShuffle()
  {
    isShuffleMode = !isShuffleMode;
    if (isShuffleMode)
      Shuffle();

    string state = !isShuffleMode ? "disabled" : "enabled";
    Logger.Log(ELogLevel.Info, $"Shuffle has been {state}");
  }

  public void ToggleRepeat()
  {
    isRepeatMode = !isRepeatMode;

    string state = !isRepeatMode ? "disabled" : "enabled";
    Logger.Log(ELogLevel.Info, $"Repeating has been {state}");
  }

  public Track GetNextTrack()
  {
    if (currentTrackIndex + 1 < tracks.Count)
      return tracks[currentTrackIndex + 1];

    return null;
  }

  public Track GetPreviousTrack()
  {
    if (currentTrackIndex - 1 >= 0)
      return tracks[currentTrackIndex - 1];

    return null;
  }

  public Track GetCurrentTrack()
  {
    if (currentTrackIndex >= 0 && currentTrackIndex < tracks.Count)
      return tracks[currentTrackIndex];

    return null;
  }

  public void NextTrack()
  {
    if (isRepeatMode)
    {
      if (currentTrackIndex == tracks.Count - 1)
        currentTrackIndex = 0;
      else
        currentTrackIndex++;
    }
    else
      if (currentTrackIndex < tracks.Count - 1)
      currentTrackIndex++;

    if (isShuffleMode)
      Shuffle();
  }

  public void PreviousTrack()
  {
    if (currentTrackIndex > 0)
      currentTrackIndex--;

    if (isShuffleMode)
      Shuffle();
  }

  public bool EndOfPlaylist()
  {
    return currentTrackIndex == tracks.Count - 1 && !isRepeatMode;
  }

  public bool IsEmpty()
  {
    return tracks.Count == 0;
  }
}