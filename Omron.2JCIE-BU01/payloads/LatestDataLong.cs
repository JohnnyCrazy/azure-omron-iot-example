using System.Runtime.InteropServices;

namespace Omron._2JCIE_BU01.Payloads
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct LatestDataLong
  {
    // gets incremented every second
    public byte sequenceNumber;
    // 0.01 deg C
    public short temperatureRaw;
    // 0.01 %RH
    public short relativeHumidityRaw;
    // 1 lx
    public short ambientLight;

    public float temperature { get => (float)temperatureRaw / 100; }
    public float relativeHumidity { get => (float)relativeHumidityRaw / 100; }
  }
}
