# Connect to device stream

In this step we will use the `Omron.2JCIE-BU01.StreamService` project to open a bi-directional websocket channel to your device.

## Steps

1. In your Azure IoT Hub Portal, under `Shared access policies`, select `iothubowner` and copy `connection string - primary key` to your `.env` files `IOT_HUB_CONNECTION_STRING`.
1. Start `Omron.2JCIE-BU01.StreamService` via VSCode
1. Send commands like `led_on` and `led_off` via console input


[Go Back](../device-client-iothub.md)
