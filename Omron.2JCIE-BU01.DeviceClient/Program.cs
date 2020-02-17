using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

namespace Omron._2JCIE_BU01.IoTDeviceClient
{
  class Program
  {
    private static string ID_SCOPE = Environment.GetEnvironmentVariable("ID_SCOPE");
    private static string REGISTRATION_ID = Environment.GetEnvironmentVariable("REGISTRATION_ID");
    private static string INDIVIDUAL_ENROLLMENT_PRIMARY_KEY =
      Environment.GetEnvironmentVariable("INDIVIDUAL_ENROLLMENT_PRIMARY_KEY");
    private static string INDIVIDUAL_ENROLLMENT_SECONDARY_KEY =
      Environment.GetEnvironmentVariable("INDIVIDUAL_ENROLLMENT_SECONDARY_KEY");

    private static bool ENABLE_FILE_UPLOAD = Environment.GetEnvironmentVariable("ENABLE_FILE_UPLOAD") == "true";
    private static bool ENABLE_STREAMING = Environment.GetEnvironmentVariable("ENABLE_STREAMING") == "true";

    private const string GlobalDeviceEndpoint = "global.azure-devices-provisioning.net";

    static async Task<int> Main(string[] args)
    {
      Console.WriteLine("Using following credentials for registering:");
      Console.WriteLine($"-> REGISTRATION_ID: {REGISTRATION_ID}");
      Console.WriteLine($"-> INDIVIDUAL_ENROLLMENT_PRIMARY_KEY: {INDIVIDUAL_ENROLLMENT_PRIMARY_KEY}");
      Console.WriteLine($"-> INDIVIDUAL_ENROLLMENT_SECONDARY_KEY: {INDIVIDUAL_ENROLLMENT_SECONDARY_KEY}");

      using(var security = new SecurityProviderSymmetricKey(
        REGISTRATION_ID, INDIVIDUAL_ENROLLMENT_PRIMARY_KEY, INDIVIDUAL_ENROLLMENT_SECONDARY_KEY))
      {
        using(var transport = new ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly))
        {
          using(DeviceClient client = await GetDeviceClient(security, transport))
          {
            using(var sensorClient = new SensorClient(client, ENABLE_FILE_UPLOAD))
            {
              await sensorClient.Initialize();
              Console.WriteLine("Sensor Client initialized. Ready for desired props, methods, telemetry data...");

              if (ENABLE_STREAMING)
              {
                sensorClient.WaitForStreamRequest();
              }
              await Task.Delay(TimeSpan.FromHours(1));
              return 0;
            }
          }
        }
      }
    }

    private static async Task<DeviceClient> GetDeviceClient(
      SecurityProviderSymmetricKey security, ProvisioningTransportHandler transport)
    {
      var client = ProvisioningDeviceClient.Create(GlobalDeviceEndpoint, ID_SCOPE, security, transport);
      var result = await client.RegisterAsync();

      Console.WriteLine($"Registration Status: {result.Status}");
      Console.WriteLine($"AssignedHub: {result.AssignedHub}; DeviceID: {result.DeviceId}");

      if (result.Status != ProvisioningRegistrationStatusType.Assigned) return null;

      var auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, security.GetPrimaryKey());

      return DeviceClient.Create(result.AssignedHub, auth, TransportType.Mqtt);
    }
  }
}
