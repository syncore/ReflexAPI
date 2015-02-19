using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SteamInfo.Extensions;
using SteamInfo.MasterServer.Filters;

namespace SteamInfo.MasterServer
{
    public class MasterServerQuery
    {
        // Master servers
        // 208.64.200.39:27011
        // 208.64.200.65:27015
        // 208.64.200.52:27011

        //// Masterserver domain name instead of hard-coded IP modification : syncore
        //public MasterServerQuery()
        //    : this("hl2master.steampowered.com", 27011)
        //{
        //}
         public MasterServerQuery()
            : this("208.64.200.39", 27011)
        {
        }
        
        //// Masterserver domain name instead of hard-coded IP modification : syncore
        //public MasterServerQuery(string host, int port)
        //    : this(new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port))
        //{
        //}

         public MasterServerQuery(string host, int port)
             : this(new IPEndPoint(IPAddress.Parse(host), port))
         {
         }


        public MasterServerQuery(IPEndPoint masterServer)
        {
            _masterServer = masterServer;
        }

        private IPEndPoint _masterServer { get; set; }

        public List<IPEndPoint> GetServers(Filter filter, int timeout)
        {
            var udpClient = new UdpClient(_masterServer.Address.ToString(), _masterServer.Port);
            // timeout modification: syncore
            udpClient.Client.ReceiveTimeout = timeout;
            udpClient.Send(new MasterServerRequest(filter));

            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var bytes = udpClient.Receive(ref remoteIpEndPoint);
            var response = new MasterServerResponse(bytes);

            return response.Servers;
        }

        public override string ToString()
        {
            return _masterServer.ToString();
        }
    }
}