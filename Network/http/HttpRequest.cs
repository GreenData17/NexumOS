using System;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;

namespace NexumOS.Network.http
{
    public class HttpRequest : IDisposable
    {
        private TcpClient _client = new();
        private NetworkStream _stream = null;

        private string _host = "example.com";
        private string _port = "80";
        private string _path = "/";

        private string _method = "GET";
        private string _requestBody = "";
        private int _bufferSize = 2048;

        public void SetHost(string host) => _host = host;
        public void SetPort(string port) => _port = port;
        public void SetPath(string path) => _path = path;
        public void SetMethod(string method) => _method = method;
        public void SetBufferSize(int bufferSize) => _bufferSize = bufferSize;
        public NetworkStream GetStream() => _stream;

        public void Send()
        {
            if (!isConnectedToWeb())
            {
                Console.WriteLine("No internet connection!"); // TODO: Replace with an Error Manager Call
                return;
            }

            _client.Connect(GetHostIp(), int.Parse(_port));
            _stream = _client.GetStream();

            byte[] sendBuffer = new byte[_bufferSize];
            BuildRequestBody();
            byte[] requestBytes = Encoding.ASCII.GetBytes(_requestBody);
            _stream.Write(requestBytes);

            Console.WriteLine("-> Request Send! <-");
        }

        private void BuildRequestBody()
        {
            _requestBody = $"{_method} {_path} HTTP/1.1\r\nHost: {_host}\r\n\r\n";
        }

        private bool isConnectedToWeb()
        {
            if (NetworkManager.GetCurrentIp() == "0.0.0.0") return false;
            
            return true;
        }

        private string GetHostIp()
        {
            Address address;

            using (var xClient = new DnsClient())
            {
                xClient.Connect(DNSConfig.DNSNameservers[0]);
                xClient.SendAsk(_host);
                address = xClient.Receive();
                xClient.Close();
            }

            return address.ToString();
        }

        public void Dispose()
        {
            if (_client.Connected)
                _client.Close();
        }
    }
}
