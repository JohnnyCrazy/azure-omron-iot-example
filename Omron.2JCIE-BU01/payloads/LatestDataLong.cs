using System.Runtime.InteropServices;

namespace Omron._2JCIE_BU01.Payloads
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct LatestDataLong
  {
    public byte sequenceNumber; // gets incremented every second
    public short temperatureRaw; // 0.01 deg C
    public short relativeHumidityRaw; // 0.01 %RH
    public short ambientLight; // 1 lx
    public int barometricPressureRaw; // 0.001 hPa
    public short soundNoiseRaw; // 0.01 db
    public short eTVOC; // 1 ppb
    public short eCO2; // 1 ppm
    public short discomfortIndexRaw; // 0.01
    public short heatStrokeRaw; // 0.01 degC
    public VibrationInformation vibrationInformation;
    public ushort siValueRaw; // 0.1 kine
    public ushort pgaRaw; // 0.1 gal
    public ushort seismicIntensityRaw; // 0.001

    public float temperature { get => (float) temperatureRaw / 100; }
    public float relativeHumidity { get => (float) relativeHumidityRaw / 100; }
    public float barometricPressure { get => (float) barometricPressureRaw / 1000; }
    public float soundNoise { get => (float) soundNoiseRaw / 100; }
    public float discomfortIndex { get => (float) discomfortIndexRaw / 100; }
    public float heatStroke { get => (float) heatStrokeRaw / 100; }
    public float siValue { get => (float) siValueRaw / 10; }
    public float pga { get => (float) pgaRaw / 10; }
    public float seismicIntensity { get => (float) seismicIntensityRaw / 1000; }
  }

  public enum VibrationInformation : byte
  {
    NONE = 0x00,
    VIBRATION = 0x01,
    EARTHQUAKE = 0x02,
  }
}
