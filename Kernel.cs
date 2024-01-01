using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
                xClient.SendAsk("repo.greendata.dev");

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
                byte[] request = Encoding.ASCII.GetBytes("GET /nexum/test HTTP/1.1\r\nHost: repo.greendata.dev\r\n\r\n");
                stream.Write(request, 0, request.Length);
                Console.WriteLine("Request Send!");

                bool respondIsLongerThenDefault = false;
                do
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string receivedString = Encoding.ASCII.GetString(buffer, 0, bytes);
                    fullRespond.Add(receivedString);
                    Console.WriteLine("Respond Received:");

                    if(!respondIsLongerThenDefault)
                        foreach (string s in receivedString.Split("\n"))
                        {
                            if (s.Contains("Content-Length:"))
                            {
                                string numberTemp = s.Replace("Content-Length: ", "");
                                string number = "";

                                foreach (char c in numberTemp)
                                {
                                    if (char.IsDigit(c)) number += c;
                                }

                                int bufferSize;
                                int.TryParse(number, out bufferSize);
                                Console.WriteLine("length " + bufferSize);
                                Console.Write(s);

                                if (bufferSize > buffer.Length) 
                                    respondIsLongerThenDefault = true;
                            }

                            if (s.Contains("Transfer-Encoding: chunked"))
                            {
                                respondIsLongerThenDefault = true;
                                Console.WriteLine("chunk");
                            }

                            fullRespond.Add(s);
                        }

                    if (!respondIsLongerThenDefault) break;

                    if (!client.Connected) break;


                } while (true) ;

                Console.WriteLine("Finished receiving!");

                fullRespond.RemoveAt(fullRespond.Count - 1);

                bool headerPassed = false; // small info -> the header is 18 lines long + 1 for the seperation line
                string fullText = "";
                for (int i = 0; i < fullRespond.Count; i++)
                {
                    if (!headerPassed && string.IsNullOrWhiteSpace(fullRespond[i]))
                    {
                        headerPassed = true;
                        continue;
                    }

                    if(!headerPassed) continue;


                    fullText += fullRespond[i];
                    if (i < fullRespond.Count - 1)
                    {
                        fullText += "\r\n";
                    }
                }

                File.Delete(@"0:\package-info.conf");
                File.WriteAllBytes(@"0:\package-info.conf", Encoding.ASCII.GetBytes(fullText));
                Console.WriteLine("FileCreated!");

                Console.WriteLine("====================");
                Console.WriteLine(File.ReadAllText(@"0:\package-info.conf"));
                Console.WriteLine("====================");
            }


            //foreach (string file in Directory.GetFiles(@"0:\"))
            //{
            //    Console.WriteLine(file);
            //}

            //Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            //Console.Write("Input: ");
            //var input = Console.ReadLine();
            //Console.Write("Text typed: ");
            //Console.WriteLine(input);
        }

        
    }
}
