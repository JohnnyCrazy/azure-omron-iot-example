using System.Runtime.InteropServices;

namespace Omron._2JCIE_BU01.Payloads
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct DeviceInformation
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] modelNumber;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] serialNumber;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] firmwareVersion;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] hardwareRevision;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] manufactureName;
  }
}
