using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Networking
{
    /// <summary>
    /// Lays out higher level functions for transporting packets and various information
    /// </summary>
    public interface ITransportLayer
    {
        public Queue<Packet> ReceivePackets();   
        public void SendPacket(Packet packet);

        // Address is a string because different protocols interpret different connection types!
        public void Connect(string address);

        public Packet CreatePacket() => new Packet(this);
    }
}
