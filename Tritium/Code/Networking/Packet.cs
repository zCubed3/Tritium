using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Networking
{
    /// <summary>
    /// Returned by the networking system or manually created, this is a buffer is used to send messages around
    /// </summary>
    public class Packet : IDisposable
    {
        protected ITransportLayer transportLayer;
        protected List<byte> data = new List<byte>();

        public IReadOnlyList<byte> Data => data;

        public Packet(ITransportLayer transportLayer)
        {
            this.transportLayer = transportLayer;
        }

        public Packet(ITransportLayer transportLayer, byte[] data) : this(transportLayer)
        {
            this.data.AddRange(data);
        }

        public void Write(params byte[] bytes)
        {
            data.AddRange(bytes);
        }

        public void Dispose()
        {
            transportLayer.SendPacket(this);
        }
    }
}
