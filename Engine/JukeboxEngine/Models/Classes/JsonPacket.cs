using Newtonsoft.Json;
using JukeboxEngine.Interfaces;

namespace JukeboxEngine.Models.Classes;

public class JsonPacket(string name, object? data = null) : IJsonPacket
{
  public string Event { get; set; } = name;
  public object Response { get; set; } = data!;

  public string ToJson()
  {
    return JsonConvert.SerializeObject(this);
  }
}