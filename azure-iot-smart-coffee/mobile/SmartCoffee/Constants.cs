﻿using System;

namespace SmartCoffee
{
    // TODO #1: Replace with your Azure IoT Hub settings.
    public class Constants
    {
        public static string HubName = "xamarin-minihack-smartcoffee";
        // Example: smartCoffeeHub
        public static string SharedAccessSignature = "SharedAccessSignature sr=xamarin-minihack-smartcoffee.azure-devices.net&sig=sMFLmoxJWbwsx4pdILaQw%2BvrkQ0uTdYPi7gUtG6XbaQ%3D&se=1493320963&skn=iothubowner";
        // Example: SharedAccessSignature sr=PierceHub.azure-devices.net&sig=pfRbp%2B43Hlt96rJCLJvpynga%2B8EkfMD2A28x8IEXABU%3D&se=1492831585&skn=iothubowner

        // These values are calculated - do not change. :)
        public static string HubHostName = $"{HubName}.azure-devices.net";
        public static string DeviceId = "coffeeMaker";
        public static string Username = $"iothubowner@sas.root.{HubName}";
        public static string Password = SharedAccessSignature;
        public static string RecipientLocation = $"{HubName}.azure-devices.net/messages/devicebound";
    }
}

