using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Networking
{
    /// <summary>
    /// Marks a field as "networked", meaning when type update payloads are sent, the field will update
    /// The type must register itself in the networking type cache for payloads to be sent and recieved
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NetAttribute : Attribute
    {
        
    }
}
