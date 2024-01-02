using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NexumOS.Network.http
{
    public class HttpRespond
    {
        private NetworkStream _stream;
        private int _BufferSize = 2048;

        private List<byte> _receivedBytes = new List<byte>();

        private int contentLength = 0;

        public HttpRespond(NetworkStream stream)
        {
            _stream = stream;
            byte[] buffer = new byte[_BufferSize];

            bool isFirstLoop = true;
            bool responseContinues = false;
            do
            {
                stream.Read(buffer, 0, buffer.Length);

                if (isFirstLoop)
                {
                    HandleHeader(buffer);

                    if (contentLength == 0) return;

                    isFirstLoop = false;
                }

                foreach (byte receivedByte in buffer)
                {
                    _receivedBytes.Add(receivedByte);
                }

                if (contentLength > buffer.Length)
                    responseContinues = true;
                else
                    responseContinues = false;

                if(!responseContinues) break;

                if(!stream.Socket.Connected) break;

            } 
            while (true);
        }

        private void HandleHeader(byte[] buffer)
        {
            string headerPart = Encoding.ASCII.GetString(buffer);
            string[] headerPartArray = headerPart.Split("\r\n");

            foreach (string line in headerPartArray)
            {
                if (line.Contains("Content-Length:"))
                {
                    string numberTemp = line.Replace("Content-Length: ", "");
                    string number = "";

                    foreach (char c in numberTemp)
                    {
                        if (char.IsDigit(c)) number += c;
                    }

                    contentLength = int.Parse(number);
                }
                else if (line.Contains("Transfer-Encoding: chunked"))
                {
                    Console.WriteLine("Transfer-Encoding chunked not Implemented yet.");
                }
            }
        }

        public void ReceiveFile(string path)
        {
            //if (File.Exists(path))
            //    File.Delete(path);

            Console.WriteLine("Attempting to Write File... [ " + path + " ]");

            byte[] bytesToWrite = _receivedBytes.ToArray();
            //File.WriteAllBytes(path, bytesToWrite);
            try
            {
                File.WriteAllText(path, "hello");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("File Written!");
        }
    }
}
