using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            string eventHubName = "fifthplayhub";
            string eventHubConnectionString = "Endpoint=sb://fifthplayeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/kgW5Bx1QSn5D/3rAabzO1jz0czT3++kmoHnmEQrjag=";
            string storageAccountName = "fifthplayeventhubstorage";
            string storageAccountKey = "hefs49XhbiVUwf8kyRQYYVgeGx9eFK1R+qDsovU1Qucp3TcJjiCfRORODwsWCImLRIrJqot8ynijp6cykwSX9Q==";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>(options).Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
