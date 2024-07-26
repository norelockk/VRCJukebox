namespace JukeboxEngine.Interfaces;

public interface IFileService
{
  void SetupDirectories();
  bool CreateMediaDirectory(string basePath, string directoryName);
}