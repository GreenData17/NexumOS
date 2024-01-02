using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.Config;

namespace NexumOS.Network
{
    public static class NetworkManager
    {
        private static string _currentIp = "0.0.0.0";

        public static string GetCurrentIp() => _currentIp;

        public static void RequestIpAddress()
        {
            using (var xClient = new DHCPClient())
            {
                /** Send a DHCP Discover packet **/
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
            }

            _currentIp = NetworkConfiguration.CurrentAddress.ToString();
        }
    }
}
