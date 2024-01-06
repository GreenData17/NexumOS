using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using NexumOS.Network;
using NexumOS.System;
using Sys = Cosmos.System;

namespace NexumOS
{
    public class Kernel : Sys.Kernel
    {
        CosmosVFS fileSystem = new();
        private FileLoader fileLoader = new();
        private bool textMode = true;

        // temp vars
        private Process proc;

        protected override void BeforeRun()
        {
            //Console.Clear();

            VFSManager.RegisterVFS(fileSystem);
            File.WriteAllText(@"0:\Hello.txt", "Hello World!");
            Console.WriteLine(File.ReadAllText(@"0:\Hello.txt"));

            NetworkManager.RequestIpAddress();
            Console.WriteLine("IP: " + NetworkManager.GetCurrentIp());

            fileLoader.Load();
            // Console.WriteLine(File.ReadAllText(@"0:\test.bin"));
            proc = new Process(@"0:\test.bin");

            Sys.MouseManager.ScreenHeight = (uint)Console.WindowHeight;
            Sys.MouseManager.ScreenWidth = (uint)Console.WindowWidth;
            Sys.MouseManager.MouseSensitivity = 0.1f;
            proc.PrintCode();
        }

        protected override void Run()
        {
            //Console.BackgroundColor = ConsoleColor.White;
            //Console.SetCursorPosition((int)Sys.MouseManager.X, (int)Sys.MouseManager.Y);
            //Console.Write(" ");
            proc.Execute();
        }

        
    }
}
