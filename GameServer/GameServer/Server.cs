using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Xml.Schema;

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> m_clients = new Dictionary<int, Client>();
        private static TcpListener m_tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers= maxPlayers;
            Port= port;

            Console.WriteLine("Starting server...");

            InitializeServerData();

            m_tcpListener = new TcpListener(IPAddress.Any, Port);
            m_tcpListener.Start();
            m_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = m_tcpListener.EndAcceptTcpClient(result);
            m_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 0; i <= MaxPlayers; ++i)
            {
                if (m_clients[i].m_tcp.socket == null)
                {
                    m_clients[i].m_tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for (int i = 0; i <= MaxPlayers; ++i)
            {
                m_clients.Add(i, new Client(i));
            }
                
        }
    }
}
