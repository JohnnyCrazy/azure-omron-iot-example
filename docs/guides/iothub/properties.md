# Update device properties

In this step we will alter the device property `interval`. We will change it from the default value of 10 seconds to every 30 seconds.

## Steps

1. (Only required once) Using VSCode, run `Select IoT Hub` via `CTRL + SHIFT + P` and select your IoT Hub
1. On the left side panel `Azure IoT Hub`, find your device and right click -> `Edit Device Twin`
1. Edit the object `properties.desired` and add the following object:

```json
"interval": {
  "value": 30000
}
```

4. Right click -> `Update Device Twin`
1. Verify new interval by looking at the sensors blinking interval.

[Go Back](../device-client-iothub.md)
