# Omron.2JCIE-BU01.DeviceClient

A console project, which connects to the IoT Hub using a Device Provision Service. It then sends (message and file upload) sensor telemetry in a configurable interval, waits for streaming requests and allows direct methods to be called.

*at the moment, only tested on windows*

## Features

### Telemetry

With a configurable interval, the project sends its current sensor data to the IoTHub, where it can be further processed. It's sent as a JSON payload and is IoT Central compatible.

### File Upload

If `ENABLE_FILE_UPLOAD=true`, the project also uploads a file in addition to sending the telemetry. The file is put in an azure blob storage, attached to the IoT Hub.

### Properties

The telemetry/file-upload interval is configured by a property `interval`. If this property is changed by the IoTHub, the client automatically adapts without a restart and uses its new value.

### Streaming (WebSockets)

It's possible to connect to the device client via a [device stream](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview). It allows to open a secure bi-directional channel without any port forwarding. In this example, the connection is used to process commands `led_on` and `led_off`. See [Omron.2JCIE-BU01.StreamService](../Omron.2JCIE-BU01.StreamService/README.md) for a connecting client implementation.
