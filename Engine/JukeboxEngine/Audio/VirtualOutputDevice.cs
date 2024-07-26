using JukeboxEngine.Enums;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace JukeboxEngine.Audio;

public class VirtualOutputDevice : IMMNotificationClient
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

  public void OnDeviceStateChanged(string deviceId, DeviceState newState)
  {
    Console.WriteLine($"Device state changed: DeviceId={deviceId}, NewState={newState}");
  }

  public void OnDeviceAdded(string pwstrDeviceId)
  {
    Console.WriteLine($"Device added: DeviceId={pwstrDeviceId}");
  }

  public void OnDeviceRemoved(string deviceId)
  {
    Console.WriteLine($"Device removed: DeviceId={deviceId}");
  }

  public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
  {
    Console.WriteLine($"Default device changed: Flow={flow}, Role={role}, DefaultDeviceId={defaultDeviceId}");
  }

  public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
  {
    // Unused
  }
}