using System;

// INSERT using statements below here
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace CaveDevice
{
    class Program
    {
        // INSERT variables below here
        private static DeviceClient deviceClient;
        private readonly static string connectionString = "HostName=iot-az220-training-bmv220202.azure-devices.net;DeviceId=sensor-th-0001;SharedAccessKey=oCYptTO2rOdwp91+RzGht554PngrBZLRL9kwY+HuVS4=";
        // INSERT Main method below here
        static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub C# Simulated Cave Device. Ctrl-C to exit.\n");
            // Connect to the IoT hub using the MQTT protocol
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
        // INSERT SendDeviceToCloudMessagesAsync method below here
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Create an instance of our sensor
            var sensor = new EnvironmentSensor();
            while (true)
            {
                // read data from the sensor
                var currentTemperature = sensor.ReadTemperature();
                var currentHumidity = sensor.ReadHumidity();
                var messageString = CreateMessageString(currentTemperature, currentHumidity);
                // create a byte array from the message string using ASCII encoding
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");
                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                await Task.Delay(1000);
            }
        }

        // INSERT CreateMessageString method below here
        private static string CreateMessageString(double currentTemperature, double currentHumidity)
        {
            // Create an anonymous object that matches the data structure we wish to send
            var telemetryDataPoint = new
            {
                temperature = currentTemperature,
                humidity = currentHumidity
            };
            // Create a JSON string from the anonymous object
            return JsonConvert.SerializeObject(telemetryDataPoint);
        }
    }

    // INSERT EnvironmentSensor class below here
    internal class EnvironmentSensor
    {
        // Initial telemetry values
        double minTemperature = 20;
        double minHumidity = 60;
        Random rand = new Random();
        internal EnvironmentSensor()
        {
            // device initialization could occur here
        }
        internal double ReadTemperature()
        {
            return minTemperature + rand.NextDouble() * 15;
        }
        internal double ReadHumidity()
        {
            return minHumidity + rand.NextDouble() * 20;
        }
    }

}
