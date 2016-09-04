using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Xml;

namespace GatewayGenerator
{
    class Program
    {

        static string eventHubName = "fifthplayhub";
        static string connectionString = "Endpoint=sb://fifthplayeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/kgW5Bx1QSn5D/3rAabzO1jz0czT3++kmoHnmEQrjag=";

        static void Main(string[] args)
        {
            int gateways;
            int waitTime;

            Console.Write("Number of Gateways to generate in 15 min: ");
            gateways = int.Parse(Console.ReadLine());
            waitTime = 900000 / gateways;

            Console.WriteLine("Press Enter to start ");
            Console.ReadLine();

            generateXML(waitTime);
        }

        static private void generateXML(int waitTime)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
            Random rnd = new Random();

            while (true)
            {
                int numberPlugs = rnd.Next(1, 8);
                Console.WriteLine(CreateAndSendXDocument(numberPlugs, eventHubClient));
                Console.WriteLine("\n");
                System.Threading.Thread.Sleep(waitTime);
            }
        }

        static private XDocument CreateAndSendXDocument(int items, EventHubClient hub)
        {

            object[] elements = new object[items];

            Random rnd = new Random();
            for (int i = 0; i < items; i++)
            {
                elements[i] = new XElement("Plug",
                new XElement("PlugUUID", Guid.NewGuid()),
                new XElement("Log", GenerateLogValue(rnd.Next(1, 3), rnd)));
            }

            XDocument xml = new XDocument(new XElement("EnergyLog", new XElement("Plugs", elements)));

            SendToEventHubs(hub, xml);

            return xml;
        }

        static private string GenerateLogValue(int items, Random rnd)
        {
            string log;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items; i++)
            {
                sb.Append(DateTime.Today.ToString("yyyyMMdd"));
                sb.Append("T");
                sb.Append(DateTime.Now.ToString("HHmmss"));
                sb.Append(";");
                sb.Append(rnd.Next(10, 99).ToString());
                sb.Append(".");
                sb.Append(rnd.Next(10, 99).ToString());
                if (i != items - 1)
                {
                    sb.Append("|");
                }
            }

            log = sb.ToString();
            return log;
        }

        static void SendToEventHubs(EventHubClient hub, XDocument xml)
        {
                Stream stream = new MemoryStream();  // Create a stream
                xml.Save(stream);      // Save XDocument into the stream
                stream.Position = 0;
                
               hub.Send(new EventData(stream));
        }

    }
}
