using System;
using System.Threading.Tasks;
using Omron._2JCIE_BU01.Payloads;

namespace Omron._2JCIE_BU01.Example
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var sensor = new Omron2JCI_BU01("COM3");
      while (true)
      {
        var data = await sensor.GetLatestData();
        Console.WriteLine("--- Sensor Data ---");
        Console.WriteLine($"-> Sequence Number     : {data.sequenceNumber}");
        Console.WriteLine($"-> Temperature         : {data.temperature} C°");
        Console.WriteLine($"-> Relative Humidity   : {data.relativeHumidity} %RH");
        Console.WriteLine($"-> Ambient Light       : {data.ambientLight} LX");
        Console.WriteLine($"-> Barometric Pressure : {data.barometricPressure} hPa");
        Console.WriteLine($"-> Sound Noise         : {data.soundNoise} db");
        Console.WriteLine($"-> eTVOC               : {data.eTVOC} ppb");
        Console.WriteLine($"-> eCO2                : {data.eCO2} ppm");
        Console.WriteLine($"-> Discomfort Index    : {data.discomfortIndex}");
        Console.WriteLine($"-> Heat Stroke         : {data.heatStroke} C°");
        Console.WriteLine($"-> Vibration Info      : {data.vibrationInformation}");
        Console.WriteLine($"-> SI Value            : {data.siValue} knie");
        Console.WriteLine($"-> PGA                 : {data.pga} gal");
        Console.WriteLine($"-> Seismic Intensity   : {data.seismicIntensity}");
        await Task.Delay(5_000);
      }
    }
  }
}
