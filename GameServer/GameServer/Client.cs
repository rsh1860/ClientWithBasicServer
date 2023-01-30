using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Client
    {
        public static int BufferSize = 4096;
        public int m_id;
        public TCP m_tcp;

        public Client(int clientId)
        {
            m_id = clientId;
            m_tcp = new TCP(m_id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;

            private NetworkStream stream;
            private byte[] receiveBuffer;

            public TCP(int id)
            {
                this.id = id;
            }

            public void Connect(TcpClient socket)
            {
                this.socket= socket;
                this.socket.ReceiveBufferSize = BufferSize;
                this.socket.SendBufferSize = BufferSize;

                stream = this.socket.GetStream();

                receiveBuffer = new byte[BufferSize];

                stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving TCP data : {ex}");
                }
            }
        }
    }
}
