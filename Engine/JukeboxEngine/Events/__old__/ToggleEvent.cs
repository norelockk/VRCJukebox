// using NAudio.Wave;
// using JukeboxEngine.Utils;
// using JukeboxEngine.Models.Enums;

// namespace JukeboxEngine.Events;

// public class ToggleEvent
// {
//   public bool Process(string action)
//   {
//     if (action.IsNullOrEmpty() || action.IsNullOrWhiteSpace())
//       return false;

//     switch (action)
//     {
//       case "Play":
//         {
//           bool state = Core.Instance.Player.PlaybackState == PlaybackState.Playing;

//           if (Core.Instance.Player.PlaybackState != PlaybackState.Stopped)
//           {
//             if (!state)
//               Core.Instance.Player.Play();
//             else
//               Core.Instance.Player.Pause();
//           }
//           else
//             Core.Instance.Player._Play();

//           Logger.Log(LogLevel.Info, $"Player {(!state ? "is playing" : "has been paused")}");
//           return true;
//         }

//       case "Repeat":
//         {
//           bool state = Core.Instance.Player.Playlist.isRepeatMode == true;

//           Core.Instance.Player.Playlist.ToggleRepeat();
//           Logger.Log(LogLevel.Info, $"Player playlist {(!state ? "repeating" : "repeat disabled")}");
//           return true;
//         }

//       default:
//         {
//           Logger.Log(LogLevel.Warning, $"Unknown toggle action: {action}");
//           return false;
//         }
//     }
//   }
// }