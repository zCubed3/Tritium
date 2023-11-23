using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tritium.Networking
{
    // TODO: Make this run on another thread?
    // TODO: Ensure determinism between clients?
    public class UDPLayer : ITransportLayer, IDisposable    
    {
        protected UdpClient client;

        // UDP doesn't receive one message at a time, rather it gets a whole buffer at once
        // Therefore a malformed message can break the server!
        // So we instead expect UDP packets to be shaped like this
        // 
        // ushort length;
        // byte[] data;
        //

        protected const string UDP_ROOT = "udp://";
        protected bool connected = false;
        protected IPEndPoint endPoint = null;

        protected BinaryWriter binaryWriter;
        protected Queue<Packet> packets = new Queue<Packet>();

        protected Thread receiveThread;

        public UDPLayer()
        {
            client = new UdpClient();
            receiveThread = new Thread(() => 
            {
                while (true)
                {
                    if (endPoint != null)
                    {
                        lock (client)
                        {
                            lock (endPoint)
                            {
                                byte[] packet = client.Receive(ref endPoint);
                                packets.Enqueue(new Packet(this, packet));
                            }
                        }
                    }
                }
            });

            receiveThread.Start();
        }

        public void Dispose()
        {
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Disconnect(true);
            client.Dispose();
        }

        public void Connect(string address)
        {
            // UDP addresses are composed like this
            // udp://IP:PORT

            if (address.StartsWith(UDP_ROOT))
            {
                string strip = address.Replace(UDP_ROOT, "");

                string[] tokens = strip.Split(":");

                if (tokens.Length != 2)
                    throw new ArgumentOutOfRangeException($"When processing address '{address}', token count != 2");

                lock (client)
                {
                    endPoint = new IPEndPoint(IPAddress.Parse(tokens[0]), int.Parse(tokens[1]));
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    lock (endPoint)
                    {
                        client.Client.Bind(endPoint);
                        client.Connect(endPoint);
                    }
                }
            }
        }

        // To prevent thread locking we receive message on another thread and this just returns the list
        public Queue<Packet> ReceivePackets()
        {
            lock (packets)
            {
                Queue<Packet> copy = new Queue<Packet>(packets);
                packets.Clear();

                return copy;
            }
        }

        public void SendPacket(Packet packet)
        {
            // TODO: Queue messages if the connection is interrupted but not completely lost yet
            if (endPoint != null)
            {
                var data = packet.Data;

                ushort len = Convert.ToUInt16(data.Count);
                List<byte> bytes = new List<byte>();

                //bytes.AddRange()

                // TODO: Faster sending?
                client.Send(packet.Data.ToArray());
            }
        }
    }
}
