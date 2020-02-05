using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omron._2JCIE_BU01.Payloads;

namespace Omron._2JCIE_BU01.IoTDeviceClient
{
  class SensorClient : IDisposable
  {
    private DeviceClient _client;
    private Omron2JCI_BU01 _sensor;
    private Timer _timer;
    private WebSocketHandler _webSocketHandler;

    private bool _enableFileUpload;
    private bool _enableStreaming;
    private string _port;
    private int _interval;
    public int Interval
    {
      get => _interval;
      set
      {
        _timer.Change(1_000, value);
        _interval = value;
      }
    }

    private const int DEFAULT_INTERVAL = 10_000;

    public SensorClient(DeviceClient client, bool enableFileUpload, bool enableStreaming)
    {
      _client = client;
      _enableFileUpload = enableFileUpload;
      _enableStreaming = enableStreaming;

      _timer = new Timer(this.OnTimerElapsed);
      _webSocketHandler = new WebSocketHandler(_client);
    }

    private async void OnTimerElapsed(object state)
    {
      await _sensor.SetLED(true, 255, 255, 255);
      // built in delay
      LatestDataLong data = await _sensor.GetLatestData();
      byte[] jsonData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

      // Send a message
      var msg = new Message(jsonData);
      await _client.SendEventAsync(msg);

      if (_enableFileUpload)
      {
        string time = DateTime.UtcNow.ToString("yyyy/MM/dd/HH_mm_ss");
        await _client.UploadToBlobAsync($"{time}.json", new MemoryStream(jsonData));
      }
      await Task.Delay(1000);
      await _sensor.SetLED(false, 255, 255, 255);
    }

    public async Task Initialize()
    {
      await _client.OpenAsync();
      await _client.SetMethodHandlerAsync("read", this.OnReadMethod, null);

      var twin = await _client.GetTwinAsync();
      var desired = twin.Properties.Desired;

      Console.WriteLine(desired.ToJson());

      if (!desired.Contains("port"))
        _port = "COM3";
      else
        _port = desired["port"].value;

      try
      {
        _sensor = new Omron2JCI_BU01(_port);
        var deviceInformation = await _sensor.GetDeviceInformation();

        if (!desired.Contains("interval"))
          Interval = DEFAULT_INTERVAL;
        else
          Interval = desired["interval"].value;

        await UpdateReportedProperties(desired, deviceInformation);
        await _client.SetDesiredPropertyUpdateCallbackAsync(this.OnDesiredPropertyUpdated, null);
      }
      catch (System.IO.FileNotFoundException)
      {
        Console.WriteLine($"Port {_port} invalid - No connection possible");
        Environment.Exit(1);
      }
    }

    public async Task WaitForStreamRequest()
    {
      using (var cancelToken = new CancellationTokenSource(TimeSpan.MaxValue))
      {
        DeviceStreamRequest req = await _client.WaitForDeviceStreamRequestAsync(cancelToken.Token);
        if (req != null)
          await _webSocketHandler.HandleStreamRequest(_sensor, req, cancelToken);
      }
    }

    public async Task UpdateReportedProperties(TwinCollection desired, DeviceInformation? deviceInfo = null)
    {
      Interval = desired.Contains("interval") ? desired["interval"].value : DEFAULT_INTERVAL;

      TwinCollection reported = new TwinCollection();
      SetReportedProperty(reported, desired, "interval", _interval);
      if (deviceInfo != null)
      {
        reported["manufacturer"] = deviceInfo?.manufactureName;
        reported["model"] = deviceInfo?.modelNumber;
        reported["swVersion"] = deviceInfo?.firmwareVersion;
        reported["serialNumber"] = deviceInfo?.serialNumberRaw;
      }
      await _client.UpdateReportedPropertiesAsync(reported);
    }

    private void SetReportedProperty<T>(TwinCollection reported, TwinCollection desired, string key, T value)
    {
      reported[key] = new
      {
        value = value,
        status = "completed",
        desiredVersion = desired["$version"],
        message = "Processed"
      };
    }

    public async Task OnDesiredPropertyUpdated(TwinCollection desired, object userContext)
    {
      await UpdateReportedProperties(desired);
    }

    public async Task<MethodResponse> OnReadMethod(MethodRequest request, object userContext)
    {
      var data = await _sensor.GetLatestData();
      return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), 0);
    }

    public void Dispose()
    {
      _timer.Dispose();
      _sensor?.Dispose();
    }
  }
}

// {
//   { "port", _port},
//   { "interval", new JObject
//     {
//       {"value", _interval},
//       {"status", "completed"},
//       {"desiredVersion", desired.Version},
//       {"message", "Processed"}
//     }
//   },
//   {
//     "device_information", new JObject
//     {
//       { "model_number", Encoding.UTF8.GetString(deviceInformation.modelNumber) },
//       { "serial_number", Encoding.UTF8.GetString(deviceInformation.serialNumber) },
//       { "firmware_version", Encoding.UTF8.GetString(deviceInformation.firmwareVersion) },
//       { "hardware_revision", Encoding.UTF8.GetString(deviceInformation.hardwareRevision) },
//       { "manufacture_name", Encoding.UTF8.GetString(deviceInformation.manufactureName) }
//     }
//   }
// };
