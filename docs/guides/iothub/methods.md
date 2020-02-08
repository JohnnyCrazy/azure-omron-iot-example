# Invoke device methods

In this step we will call a method on the device. In this case, we will request the current sensor data.

## Steps

1. (Only required once) Using VSCode, run `Select IoT Hub` via `CTRL + SHIFT + P` and select your IoT Hub
1. On the left side panel `Azure IoT Hub`, find your device and right click -> `Invoke Device Direct Method`
1. Method Name: `read`
1. Method Payload: `{}`
1. Verify JSON Sensor data in output


[Go Back](../device-client-iothub.md)
