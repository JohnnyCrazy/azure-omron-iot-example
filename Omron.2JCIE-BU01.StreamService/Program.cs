using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Omron._2JCIE_BU01.StreamService
{
  class Program
  {
    private static string IOT_HUB_CONNECTION_STRING = Environment.GetEnvironmentVariable("IOT_HUB_CONNECTION_STRING");
    private static string DEVICE_ID = Environment.GetEnvironmentVariable("REGISTRATION_ID");
    static async Task Main(string[] args)
    {
      Console.WriteLine("Starting service...");
      using (ServiceClient client = ServiceClient.CreateFromConnectionString(IOT_HUB_CONNECTION_STRING))
      {
        var req = new DeviceStreamRequest(streamName: "TestStream");
        DeviceStreamResponse res = await client.CreateStreamAsync(DEVICE_ID, req);

        if (res.IsAccepted)
        {
          using (var cancelToken = new CancellationTokenSource())
          {
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
              webSocket.Options.SetRequestHeader("Authorization", $"Bearer {res.AuthorizationToken}");
              await webSocket.ConnectAsync(res.Url, cancelToken.Token);

              string line = "";
              while (line.ToLower() != "q")
              {
                Console.Write("Your message: ");
                line = Console.ReadLine();

                await RespondString(webSocket, cancelToken, line);
                var response = await ReceiveString(webSocket, cancelToken);
                Console.WriteLine($"--> Received message: \"{response}\"");
              }
              await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancelToken.Token);
            }
          }
        }
        await client.CloseAsync();
      }
    }

    private static async Task<string> ReceiveString(ClientWebSocket webSocket, CancellationTokenSource cancelToken)
    {
      byte[] buffer = new byte[1024];
      var recv = await webSocket.ReceiveAsync(buffer, cancelToken.Token);
      if (recv.MessageType == WebSocketMessageType.Close)
      {
        Console.WriteLine("[STREAMING] Closed");
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancelToken.Token);
        return null;
      }
      return Encoding.UTF8.GetString(buffer, 0, recv.Count);
    }

    private static async Task RespondString(ClientWebSocket webSocket, CancellationTokenSource cancelToken, String response)
    {
      var buffer = Encoding.UTF8.GetBytes(response);
      await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, cancelToken.Token);
    }
  }
}
