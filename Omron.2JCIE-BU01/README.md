# Omron.2JCIE-BU01

A library project, which allows reading and writing to the 2JCIE-BU01 Sensor via a USB Serial connection. While not every write/read method is implemented, reading sensor data and setting the LED Color is possible.

**Drivers have to be installed, so the USB Serial Connection can be established, see links section!**

*at the moment, only tested on windows*

## Sample Code

### Read latest sensor data

```c#
static async Task Main(string[] args)
{
  using (var sensor = new Omron2JCI_BU01("COM3"))
  {
    LatestDataLong data = await sensor.GetLatestData();
    Console.WriteLine($"Temperature: ${data.temperature} °C");
  }
}
```

### Turn on RGB LED

```c#
static async Task Main(string[] args)
{
  using (var sensor = new Omron2JCI_BU01("COM3"))
  {
    await sensor.SetLED(true, 255, 0, 0);
  }
}
```

## Links

* [Drivers & Instructions](https://www.components.omron.com/sensors/iot-sensors/enviorment-sensors/2jcie_bu01_usb-driver/download)
