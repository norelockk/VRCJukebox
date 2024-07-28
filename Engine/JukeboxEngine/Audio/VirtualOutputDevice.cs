using JukeboxEngine.Enums;
using NAudio.CoreAudioApi;

namespace JukeboxEngine.Audio;

public class VirtualOutputDevice
{
  private readonly MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();

  public VirtualOutputDevice()
  {
    Logger.Log(ELogLevel.Debug, $"VirtualOutputDevice(): constructor");
  }

  private int deviceIndex = -1;
  private readonly string deviceName = "CABLE Input (VB-Audio Virtual Cable)";

  private protected MMDeviceCollection GetAllDevices()
  {
    return deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
  }

  // fixme: proper gathering an device index for NAudio library
  // because it dosen't like the basic indexes (from 0's, i mean it liked before but now idfk what happened)  
  public int GetIndex()
  {
    if (deviceIndex == -1)
    {
      var devices = GetAllDevices();

      for (int i = 0; i < devices.Count; i++)
      {
        var device = devices[i];

        if (device.FriendlyName == deviceName)
        {
          Logger.Log(ELogLevel.Debug, $"Found supported output device at index {i}");
          deviceIndex = i;
          return i;
        }
      }

      throw new Exception("Virtual output device not available");
    }
    else return deviceIndex;
  }
}