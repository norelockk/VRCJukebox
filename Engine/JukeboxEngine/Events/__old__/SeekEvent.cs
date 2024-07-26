// using JukeboxEngine.Models.Enums;
// using NAudio.Wave;
// using JukeboxEngine.Utils;

// namespace JukeboxEngine.Events;

// public class SeekEvent
// {
//   public bool Process<T>(string action, T value) where T : IComparable<T>, IConvertible
//   {
//     if (string.IsNullOrWhiteSpace(action))
//       return false;

//     switch (action)
//     {
//       case "Volume":
//         {
//           float vol = NumberUtils.MathFixture(Convert.ToSingle(value), 1);

//           Core.Instance.Player.Volume = vol;
//           Logger.Log(LogLevel.Info, $"Player volume set to {vol}");
//           return true;
//         }

//       case "Position":
//         {
//           if (Core.Instance.Player.PlaybackState != PlaybackState.Playing)
//             return false;

//           var songLength = Core.Instance.Player.GetCurrentLength();

//           double pos = (double)Convert.ToSingle(value);
//           if (pos > songLength)
//             pos = songLength;

//           Core.Instance.Player.CurrentAudio.CurrentTime = TimeSpan.FromSeconds(pos);
//           Logger.Log(LogLevel.Info, $"Player current time seek set to {pos}");
//           return true;
//         }

//       default:
//         Logger.Log(LogLevel.Warning, $"Unknown seek action: {action}");
//         return false;
//     }
//   }
// }