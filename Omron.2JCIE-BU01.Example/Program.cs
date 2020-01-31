using System;
using System.Threading.Tasks;
using Omron._2JCIE_BU01;

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
        Console.WriteLine($"Temperature: {data.temperature}. Light: {data.ambientLight}");
        await Task.Delay(1000);
      }
    }
  }
}
