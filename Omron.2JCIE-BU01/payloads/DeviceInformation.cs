using System.Runtime.InteropServices;
using System.Text;

namespace Omron._2JCIE_BU01.Payloads
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct DeviceInformation
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] modelNumberRaw;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] serialNumberRaw;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] firmwareVersionRaw;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] hardwareRevisionRaw;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] manufactureNameRaw;

    public string modelNumber { get => Encoding.UTF8.GetString(modelNumberRaw); }
    public string serialNumber { get => Encoding.UTF8.GetString(serialNumberRaw); }
    public string firmwareVersion { get => Encoding.UTF8.GetString(firmwareVersionRaw); }
    public string hardwareRevision { get => Encoding.UTF8.GetString(hardwareRevisionRaw); }
    public string manufactureName { get => Encoding.UTF8.GetString(manufactureNameRaw); }
  }
}
