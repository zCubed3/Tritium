using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Tritium.Concepts;

namespace Tritium.Assets
{
    /// <summary>
    /// A package schema for a mod, called CargoPackage because it's loaded into the "CargoBay"
    /// </summary>
    public class CargoPackage
    {
        /// <summary>
        /// The name of this mod package
        /// </summary>
        public string name = "Invalid Package";

        /// <summary>
        /// A short summary describing this package
        /// </summary>
        public string description = "";

        /// <summary>
        /// What version is this mod, is a string because you can set it to anything you want!
        /// </summary>
        public string version = "0.0.0";

        /// <summary>
        /// Who are the fine people who made this mod?
        /// </summary>
        public string author = "Unknown Author";

        // TODO: Game version limits?

        /// <summary>
        /// Unique ID used to reference this mod, allows for people to extend your mods by referencing package content
        /// </summary>
        public string id = "trit.unknown.id";

        /// <summary>
        /// A list of IDs referencing other mods as dependencies
        /// Will not allow package to load unless dependencies are fuffiled
        /// </summary>
        public List<string> dependencies = new List<string>();

        // Packages have UNIX like paths, the root is /
        // This means referencing local content is via /Content.File
        // Referencing content inside other packages is via Package.id/Content.File

        /// <summary>
        /// Package.xml relative impl folder
        /// </summary>
        public string implsPath = "/Impls";
        
        /// <summary>
        /// Package.xml relative content folder
        /// </summary>
        public string contentPath = "/Content";

        /// <summary>
        /// Package.xml relative assemblies folder
        /// </summary>
        public string assembliesPath = "/Assemblies";

        /// <summary>
        /// The cached content for this loaded package, is not serialized!
        /// </summary>
        public ContentCache packageContent = new ContentCache();

        /// <summary>
        /// The loaded Impls for this package
        /// </summary>
        public Dictionary<string, Impl> packageImpls = new Dictionary<string, Impl>();


        public bool TryGetImpl(string name, out Impl impl) => packageImpls.TryGetValue(name, out impl);

        public bool TryGetImpl<T>(string name, out T impl) where T : Impl
        {
            bool state = TryGetImpl(name, out Impl temp);

            impl = temp as T;
            return state && impl != null;
        }
    }
}
