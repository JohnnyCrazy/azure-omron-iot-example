# Creating Azure IoT Services

You are going to create an IoT Hub, Azure Container Registry and a Device Provisioning Service (DPS).

## Steps

1. Login into Azure Portal
1. Create an IoT Hub using [this guide](https://docs.microsoft.com/en-us/azure/iot-dps/tutorial-set-up-cloud#create-an-iot-hub)
1. (Optional) Create Azure Container Registry using *[this guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal#create-a-container-registry)*
1. Create a Device Provisioning Service as described in *[this guide](https://docs.microsoft.com/en-us/azure/iot-dps/tutorial-set-up-cloud#create-a-device-provisioning-service-instance-and-get-the-id-scope)*
1. Link your IoT Hub to the Device Provisioning Service you've created using *[this guide](https://docs.microsoft.com/en-us/azure/iot-dps/tutorial-set-up-cloud#link-the-device-provisioning-service-to-an-iot-hub)*
1. Review supported attestation mechanisms *[here](https://docs.microsoft.com/en-us/azure/iot-dps/concepts-security)*
1. For simplicity we'll use *individual enrollments* with symmetric key. Please refer to *[this guide](https://docs.microsoft.com/en-us/azure/iot-dps/concepts-symmetric-key-attestation)* for details on how to create an enrollment for your IoT Device
