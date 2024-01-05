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
            Console.WriteLine("IP: " + NetworkConfiguration.CurrentAddress);
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
