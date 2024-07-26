namespace JukeboxEngine.Models.Classes;

public class Track
{
  public string? Id { get; init; }
  public string? Name { get; init; }
  public string? Author { get; init; }
  public string? SavePath { get; set; }
  public string? Collection { get; set; }
  public string? ArtworkPath { get; set; }
  public bool IsDownloaded { get; set; }
}
