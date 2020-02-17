using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Omron._2JCIE_BU01.IoTDeviceClient
{
  public class WebSocketHandler
  {
    private DeviceClient _client;
    public WebSocketHandler(DeviceClient client)
    {
      _client = client;
    }

    public async Task HandleStreamRequest(
      Omron2JCI_BU01 sensor,
      DeviceStreamRequest req,
      CancellationTokenSource cancelToken)
    {
      await _client.AcceptDeviceStreamRequestAsync(req, cancelToken.Token);
      using (ClientWebSocket webSocket = new ClientWebSocket())
      {
        webSocket.Options.SetRequestHeader("Authorization", $"Bearer {req.AuthorizationToken}");
        await webSocket.ConnectAsync(req.Url, cancelToken.Token);
        await ReceiveLoop(sensor, webSocket, cancelToken);
        Console.WriteLine("Receive Loop Ended");
      }
    }

    private async Task ReceiveLoop(Omron2JCI_BU01 sensor, ClientWebSocket webSocket, CancellationTokenSource cancelToken)
    {
      var recvStr = await ReceiveString(webSocket, cancelToken);
      if (recvStr == null)
        return;

      Console.WriteLine($"[STREAMING] Received: {recvStr}");
      switch (recvStr.ToLower())
      {
        case "led_on":
          await sensor.SetLED(true, 255, 255, 255);
          await RespondString(webSocket, cancelToken, "true");
          break;
        case "led_off":
          await sensor.SetLED(false, 255, 255, 255);
          await RespondString(webSocket, cancelToken, "true");
          break;
        default:
          await RespondString(webSocket, cancelToken, $"Unkown command: {recvStr}");
          break;
      }

      await ReceiveLoop(sensor, webSocket, cancelToken);
    }

    private async Task<string> ReceiveString(ClientWebSocket webSocket, CancellationTokenSource cancelToken)
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

    private async Task RespondString(ClientWebSocket webSocket, CancellationTokenSource cancelToken, String response)
    {
      var buffer = Encoding.UTF8.GetBytes(response);
      await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, cancelToken.Token);
    }
  }
}
