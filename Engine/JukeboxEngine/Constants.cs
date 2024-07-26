using System.Reflection;

namespace JukeboxEngine;

public static class Constants
{
  public static readonly string projectName = "VRChat Music Bot";
  public static readonly string projectLogo = Figgle.FiggleFonts.Standard.Render(projectName);
  public static readonly string projectVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;
}

public static class OscConstants
{
  public static readonly string OSC_PATH_CHATBOX = "/chatbox";
  public static readonly string OSC_PATH_CHATBOX_INPUT = "/chatbox/input";
  public static readonly string OSC_PATH_CHATBOX_TYPING = "/chatbox/typing";
}