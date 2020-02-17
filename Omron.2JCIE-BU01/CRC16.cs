using System;
using System.IO;

namespace Omron._2JCIE_BU01
{
  public static class CRC16
  {
    public static byte[] encode(byte[] buffer, int length = -1)
    {
      int crc = 0xFFFF;
      length = length == -1 ? buffer.Length : length;
      for (int i = 0; i < length; i++)
      {
        var c = buffer[i];
        crc = crc ^ c;
        for (int j = 0; j < 8; j++)
        {
          var carrayFlag = crc & 1;
          crc = crc >> 1;
          if (carrayFlag == 1)
            crc = crc ^ 0xA001;
        }
      }
      byte crcH = (byte)(crc >> 8);
      byte crcL = (byte)(crc & 0x00FF);
      return new[] { crcL, crcH };
    }
  }

}
