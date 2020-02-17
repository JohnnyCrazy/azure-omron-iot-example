using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Omron._2JCIE_BU01.Payloads;

namespace Omron._2JCIE_BU01
{
  public class Omron2JCI_BU01 : IDisposable
  {
    SerialPort _serialPort;

    public int ReadDelay { get; set; } = 400;
    public int WriteDelay { get; set; } = 400;
    public Omron2JCI_BU01(string port)
    {
      _serialPort = new SerialPort(port);
      _serialPort.BaudRate = 115200;
      _serialPort.Parity = Parity.None;
      _serialPort.DataBits = 8;
      _serialPort.StopBits = StopBits.One;
      _serialPort.Open();
    }

    // 0x52, 0x42, 0x0a, 0x00, 0x02, 0x11, 0x51, 1, 0x00, 0xFF, 0xFF, 0xFF

    private async Task SendCommandAsync(Command command, Address address, byte[] payload)
    {
      _serialPort.DiscardInBuffer();
      var list = new List<byte> { 0x52, 0x42 };
      list.AddRange(BitConverter.GetBytes((short)(5 + payload.Length)));
      list.Add((byte)command);
      list.AddRange(BitConverter.GetBytes((ushort)address));
      list.AddRange(payload);
      list.AddRange(CRC16.encode(list.ToArray()));

      _serialPort.Write(list.ToArray(), 0, list.Count);
      await Task.Delay(command == Command.WRITE ? WriteDelay : ReadDelay);
      if (command == Command.WRITE)
      {
        // For testing purposes, we don't care about WRITE Results, just READ results
        _serialPort.DiscardInBuffer();
      }
    }

    private async Task<T> SendCommandAsync<T>(Command command, Address address, byte[] payload) where T : struct
    {
      await SendCommandAsync(command, address, payload);

      int bytesToRead = _serialPort.BytesToRead;
      byte[] buffer = new byte[bytesToRead];
      int bytesRead = _serialPort.Read(buffer, 0, bytesToRead);

      if (bytesRead < 6)
        throw new Exception($"Invalid amount of bytes received as response: {bytesRead}");

      var header = BitConverter.ToInt16(buffer, 0);
      if (header != 0x4252)
        throw new Exception($"Unkown header received in response: {header}");

      var length = BitConverter.ToInt16(buffer, 2);
      if (length < 2)
        throw new Exception($"Invalid length in response: {length}");

      var crc = BitConverter.ToUInt16(buffer, buffer.Length - 2);
      var calculatedCrc = BitConverter.ToUInt16(CRC16.encode(buffer, buffer.Length - 2), 0);
      if (crc != calculatedCrc)
        throw new Exception($"CRC Check incorrect: {crc} != {calculatedCrc}");

      var responsePayload = buffer.Skip(4).Take(length - 2).ToArray();
      if (responsePayload[0] != (byte)command)
        throw new Exception($"Invalid response command: {responsePayload[0]}");

      var responseAddress = BitConverter.ToUInt16(responsePayload, 1);
      if (responseAddress != (ushort)address)
        throw new Exception($"Invalid response address: {responseAddress}");

      int structSize = Marshal.SizeOf<T>();
      IntPtr structBuffer = Marshal.AllocHGlobal(structSize);
      Marshal.Copy(responsePayload, 3, structBuffer, structSize);
      T ret = (T)Marshal.PtrToStructure(structBuffer, typeof(T));
      Marshal.FreeHGlobal(structBuffer);

      return ret;
    }

    public Task SetLED(bool on, byte r, byte g, byte b)
    {
      return SendCommandAsync(Command.WRITE, Address.LED_SETTING, new byte[] { (byte)(on ? 1 : 0), 0, r, g, b });
    }

    public Task<LatestDataLong> GetLatestData()
    {
      return SendCommandAsync<LatestDataLong>(Command.READ, Address.LATEST_DATA_LONG, new byte[] { });
    }

    public Task<DeviceInformation> GetDeviceInformation()
    {
      return SendCommandAsync<DeviceInformation>(Command.READ, Address.DEVICE_INFORMATION, new byte[] { });
    }

    public void Dispose()
    {
      _serialPort.Close();
      _serialPort.Dispose();
    }

    private enum Address : ushort
    {
      LED_SETTING = 0x5111,
      LATEST_DATA_LONG = 0x5021,
      DEVICE_INFORMATION = 0x180A
    }

    private enum Command : byte
    {
      READ = 0x01,
      WRITE = 0x02,
    }
  }
}
