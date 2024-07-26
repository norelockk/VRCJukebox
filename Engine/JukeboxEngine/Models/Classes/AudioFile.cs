using NAudio.Wave;

namespace JukeboxEngine.Models.Classes;

public class AudioFile : AudioFileReader
{
  public AudioFile(string path) : base(path)
  {

  }
}