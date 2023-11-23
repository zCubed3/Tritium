using System.Collections.Generic;
using System.Xml.Linq;

namespace Tritium.Assets
{
    /// <summary>
    /// The mod config file, stores load order and other settings
    /// </summary>
    public class ModConfigFile : ConfigFile
    {
        public override string Name => "ModConfig";

        public List<string> loadOrder = new List<string>()
        {
            "tritium.core.content"
        };
    }
}
