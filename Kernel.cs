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
using NexumOS.Network;
using NexumOS.Network.http;
using Sys = Cosmos.System;

namespace NexumOS
{
    public class Kernel : Sys.Kernel
    {
        CosmosVFS fileSystem = new();

        protected override void BeforeRun()
        {
            //Console.Clear();

            VFSManager.RegisterVFS(fileSystem);
            File.WriteAllText(@"0:\Hello.txt", "Hello World!");
            Console.WriteLine(File.ReadAllText(@"0:\Hello.txt"));

            NetworkManager.RequestIpAddress();

            Console.WriteLine("IP: " + NetworkConfiguration.CurrentAddress.ToString());

            using (HttpRequest request = new())
            {
                request.SetHost("repo.greendata.dev");
                request.SetPath("/nexum/base");

                request.Send();  // TODO: from this point file writing breaks!

                HttpRespond respond = new(request.GetStream());
                respond.ReceiveFile(@"0:\package-info.conf");
            }

            

            //List<string> newLines = new List<string>();
            //foreach (string line in File.ReadAllLines(@"0:\package-info.conf"))
            //{
            //    if(String.IsNullOrWhiteSpace(line)) continue;
            //    newLines.Add(line);
            //}

            //Console.WriteLine("Finished parsing!");

            //if(File.Exists(@"0:\package-info.conf"))
            //    File.Delete(@"0:\package-info.conf");

            //File.WriteAllLines(@"0:\package-info.conf", newLines);


            


            //Console.WriteLine("====================");
            //Console.WriteLine(File.ReadAllText(@"0:\package-info.conf"));
            //Console.WriteLine("====================");
            //}


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
