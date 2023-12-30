using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Sys = Cosmos.System;

namespace NexumOS
{
    public class Kernel : Sys.Kernel
    {
        CosmosVFS fileSystem = new CosmosVFS();

        protected override void BeforeRun()
        {
            VFSManager.RegisterVFS(fileSystem);

            using (var xClient = new DHCPClient())
            {
                /** Send a DHCP Discover packet **/
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
            }

            Console.WriteLine("IP: " + NetworkConfiguration.CurrentAddress.ToString());

            Address destination;

            using (var xClient = new DnsClient())
            {
                xClient.Connect(DNSConfig.DNSNameservers[0]); //DNS Server address

                /** Send DNS ask for a single domain name **/
                xClient.SendAsk("example.com");

                /** Receive DNS Response **/
                destination = xClient.Receive(); //can set a timeout value
                xClient.Close();
            }
            Console.WriteLine(destination);

            using (TcpClient client = new TcpClient())
            {
                List<string> fullRespond = new List<string>();

                client.Connect(destination.ToString(), 80);
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Connection established!");

                string messageToSend = "";

                byte[] buffer = new byte[2048];
                byte[] request = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: example.com\r\n\r\n");
                stream.Write(request, 0, request.Length);
                // stream.Flush(); // -- DO NOT FLUSH BEFORE READING, 4 DAYS WASTED. TODO: CHANGE PERSONAL DUCKUMENTATION! --
                Console.WriteLine("Request Send!");

                do
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string receivedString = Encoding.UTF8.GetString(buffer, 0, bytes);
                    fullRespond.Add(receivedString);
                    Console.WriteLine("Respond Received:");
                    Console.WriteLine(receivedString);


                    if (receivedString.Contains("</html>")) break;
                } while(true);

                // stream.Flush(); // flush doesn't work in general TT

                Console.WriteLine("Finished receiving!");
                File.WriteAllLines(@"0:\test.txt", fullRespond.ToArray());
                Console.WriteLine("FileCreated!");
            }


            foreach (string file in Directory.GetFiles(@"0:\"))
            {
                Console.WriteLine(file);
            }

            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }
    }
}
