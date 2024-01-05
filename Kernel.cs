using System;
using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using NexumOS.Network;
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
            Console.WriteLine("IP: " + NetworkManager.GetCurrentIp());
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
