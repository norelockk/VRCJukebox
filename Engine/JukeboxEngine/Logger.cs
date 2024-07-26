using System.Diagnostics;
using System.Runtime.CompilerServices;
using JukeboxEngine.Enums;

namespace JukeboxEngine;

public static class Logger
{
  private static readonly string resetColor = "\x1b[0m";
  private static readonly string infoColor = "\x1b[32m";
  private static readonly string errorColor = "\x1b[31m";
  private static readonly string debugColor = "\x1b[36m";
  private static readonly string warningColor = "\x1b[33m";
  private static readonly string namespaceColor = "\x1b[95m";

  private static string GetColoredNamespace(
      string namespaceName,
      string className,
      string memberName,
      string fileName,
      int sourceLineNumber,
      int columnNumber)
  {
    // weird method to just skip this piece of crap ikr
    string classStr = "";
    string memberStr = "";

    if (className != "Program")
      classStr = $".{className}";

    if (memberName != ".ctor")
      memberStr = $"/{memberName}";

    string prefix = "";
    string suffix = "";

#if DEBUG
    prefix = $" [{namespaceName}{classStr}{memberStr}] ";
    suffix = $"({fileName}:{sourceLineNumber}:{columnNumber})";
#endif

    return $"{namespaceColor}{prefix}{resetColor}{suffix}";
  }

  private static string GetClassName()
  {
    StackFrame frame = new StackFrame(2);
    var method = frame.GetMethod();
    return method?.DeclaringType?.Name ?? "UnknownClass";
  }

  public static void Log(
      ELogLevel level,
      string message,
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0,
      [CallerMemberName] string sourceMemberName = "")
  {
    StackFrame frame = new StackFrame(1, true);

    var method = frame.GetMethod();
    var declaringType = method?.DeclaringType;
    var namespaceName = declaringType?.Namespace ?? "UnknownNamespace";
    var className = GetClassName();
    var fileName = Path.GetFileName(sourceFilePath);
    var columnNumber = frame.GetFileColumnNumber();

    string levelColor;
    string levelString;

    switch (level)
    {
      default:
        levelColor = resetColor;
        levelString = "UNKNOWN";
        break;

      case ELogLevel.Info:
        levelColor = infoColor;
        levelString = "INFO";
        break;

      case ELogLevel.Debug:
        levelColor = debugColor;
        levelString = "DEBUG";
        break;

      case ELogLevel.Error:
        levelColor = errorColor;
        levelString = "ERROR";
        break;

      case ELogLevel.Warning:
        levelColor = warningColor;
        levelString = "WARNING";
        break;
    }

    Console.WriteLine($"{DateTime.Now} :: {levelColor}[{levelString}]{resetColor}{GetColoredNamespace(namespaceName, className, sourceMemberName, fileName, sourceLineNumber, columnNumber)}: {message}");
  }
}