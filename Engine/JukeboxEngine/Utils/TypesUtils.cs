using System.Text;
using System.Text.RegularExpressions;

namespace JukeboxEngine.Utils;

public class NumberUtils
{
  public static T MathFixture<T>(T value, T max)
  {
    double val = Convert.ToDouble(value);
    double mx = Convert.ToDouble(max);

    if (val > mx)
      val = mx;

    if (typeof(T) == typeof(long))
      return (T)(object)Convert.ToInt64(val);
    else if (typeof(T) == typeof(int))
      return (T)(object)Convert.ToInt32(val);
    else if (typeof(T) == typeof(double))
      return (T)(object)val;

    return (T)Convert.ChangeType(val, typeof(T));
  }

  public static string ConvertSeconds<T>(T totalSeconds)
  {
    double seconds;

    if (typeof(T) == typeof(long))
      seconds = Convert.ToDouble((long?)(object?)totalSeconds);
    else if (typeof(T) == typeof(int))
      seconds = Convert.ToDouble((int?)(object?)totalSeconds);
    else if (typeof(T) == typeof(double))
      seconds = Convert.ToDouble((double?)(object?)totalSeconds);
    else
      seconds = Convert.ToDouble(totalSeconds);

    TimeSpan time = TimeSpan.FromSeconds(seconds);
    string timeString = "";

    if (time.Hours > 0)
      timeString += $"{time.Hours:00}:";

    timeString += $"{time.Minutes:00}:{time.Seconds:00}";

    return timeString;
  }
}

public static class StringUtils
{
  public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

  public static bool IsNullOrWhiteSpace(this string input) => string.IsNullOrWhiteSpace(input);

  public static string ToPathSafeString(this string input) => Regex.Replace(input, "[\\/:*?\"<>|]*", "");

  public static string ToDelimitedString(this IEnumerable<string> collection) => string.Join(", ", collection);

  public static string ToCleanQueryString(this string input) => Regex.Replace(input, "^.*:\\s*", "");

  public static string UnicodeToUTF8(string from)
  {
    var bytes = Encoding.UTF8.GetBytes(from);
    return new string(bytes.Select(b => (char)b).ToArray());
  }
}