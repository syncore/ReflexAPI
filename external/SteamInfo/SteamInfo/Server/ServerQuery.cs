using System.Net;
using System.Net.Sockets;
using SteamInfo.Extensions;

namespace SteamInfo.Server
{
    public class ServerQuery
    {
        private readonly IPEndPoint _server;

        public ServerQuery(IPEndPoint server)
        {
            _server = server;
        }

        public ChallengeResponse GetChallenge(int timeout)
        {
            var bytesReceived = GetResponseBytes(new ChallengeRequest(), timeout);
            var response = new ChallengeResponse(bytesReceived);

            return response;
        }

        public PlayerResponse GetPlayers(int timeout)
        {
            var challenge = GetChallenge(timeout);
            return GetPlayers(challenge, timeout);
        }

        public PlayerResponse GetPlayers(ChallengeResponse challenge, int timeout)
        {
            var bytesReceived = GetResponseBytes(new PlayerRequest(challenge.Token), timeout);
            var response = new PlayerResponse(bytesReceived);

            return response;
        }

        public InfoResponse GetServerInfo(int timeout)
        {
            var bytesReceived = GetResponseBytes(new InfoRequest(), timeout);
            var response = new InfoResponse(bytesReceived);

            return response;
        }

        public override string ToString()
        {
            return _server.ToString();
        }

        private byte[] GetResponseBytes(IServerRequest request, int timeout)
        {
            var udpClient = new UdpClient(_server.Address.ToString(), _server.Port);
            // timeout modification: syncore
            udpClient.Client.ReceiveTimeout = timeout;
            udpClient.Send(request);

            var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var bytesReceived = udpClient.Receive(ref remoteIpEndPoint);

            return bytesReceived;
        }
    }
}