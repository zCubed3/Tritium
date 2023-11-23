using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Reflection;

using Tritium.Concepts;

namespace Tritium.Assets
{
    /// <summary>
    /// Class that helps in loading and caching packages.
    /// Used by the game to load content
    /// </summary>
    public static class CargoBay
    {
        public static CargoPackage ActivePackage = null;

        public static readonly Dictionary<string, CargoPackage> LoadedPackages = new Dictionary<string, CargoPackage>();
        public static readonly Dictionary<string, string> LoadedPackageFolders = new Dictionary<string, string>();

        private static ModConfigFile _modConfig;
        public static ModConfigFile ModConfig
        {
            get
            {
                if (_modConfig == null)
                    _modConfig = ConfigFile.Load<ModConfigFile>();

                if (_modConfig == null)
                    throw new Exception("Failed to load mod config!");

                return _modConfig;
            }
        }

        public static void LoadPackages(ref string status)
        {
            // TODO: CargoPackage contexts

            //
            // CargoBay stage 0, loads the package schemas
            //
            int stage = 0;
            foreach (string packageFolder in Directory.GetDirectories("Packages", "*", SearchOption.TopDirectoryOnly))
            {
                status = $"Stage {stage}: Loading package schema '{packageFolder}'...";

                string packageXmlPath = $"{packageFolder}/Package.xml";
                if (File.Exists(packageXmlPath))
                {
                    XDocument document = XDocument.Load(packageFolder + "/Package.xml");

                    CargoPackage package = new CargoPackage();
                    CargoXScribe.PopulateTypeFields(document.Root, ref package);

                    // TODO: Conflict resolution?
                    // TODO: Bay package IDisposable?

                    if (ModConfig.loadOrder.Contains(package.id))
                    {
                        LoadedPackages.Add(package.id, package);
                        LoadedPackageFolders.Add(package.id, packageFolder);
                    }
                }
            }

            //
            // CargoBay stage 1, loads mod assemblies
            //
            stage++;
            foreach (string id in ModConfig.loadOrder)
            {
                status = $"Stage {stage}: Loading '{id}'...";

                var package = ActivePackage = LoadedPackages[id];
                string asmPath = LoadedPackageFolders[id] + package.assembliesPath;

                if (Directory.Exists(asmPath))
                {
                    foreach (string dll in Directory.GetFiles(asmPath, "*.dll", SearchOption.AllDirectories))
                    {
                        Assembly asm = Assembly.LoadFrom(dll);
                    }
                }
            }

            //
            // CargoBay stage 2, loads the Impls
            //
            stage++;
            foreach (string id in ModConfig.loadOrder)
            {
                status = $"Stage {stage}: Loading Impls for '{id}'...";

                var package = ActivePackage = LoadedPackages[id];
                string implPath = LoadedPackageFolders[id] + package.implsPath;

                // TODO: Preload content?
                if (Directory.Exists(implPath))
                {
                    foreach (string xmlImpl in Directory.GetFiles(implPath, "*.xml", SearchOption.AllDirectories))
                    {
                        // TODO: Crash handling
                        XDocument implDocument = XDocument.Load(xmlImpl);
                        Impl.RegisterXDocument(package, implDocument);
                    }
                }
            }
        }

        public static void LoadPackages()
        {
            string _ = "";
            LoadPackages(ref _);
        }

        // TODO: Cache this?
        public static IEnumerable<Impl> GetAllImpls()
        {
            List<Impl> impls = new List<Impl>();

            foreach (CargoPackage package in LoadedPackages.Values)
                impls.AddRange(package.packageImpls.Values);

            return impls;
        }

        public static bool TryGetImpl(string package, string implName, out Impl impl)
        {
            return LoadedPackages[package].TryGetImpl(implName, out impl);
        }

        public static bool TryGetImpl<T>(string package, string name, out T impl) where T : Impl
        {
            bool state = TryGetImpl(package, name, out Impl temp);

            impl = temp as T;
            return state && impl != null;
        }

#if DEBUG
        public static string CargoBayDebugPath => "CargoBayDebug";

        public static void DumpDebugInfo()
        {
            if (!Directory.Exists(CargoBayDebugPath))
                Directory.CreateDirectory(CargoBayDebugPath);

            // We dump several files
            // Primarily we're interested in LoadedImpls.xml and ImplTypes.xml
            // Those provide good debug info on the layout of classes
            {
                XDocument loadImplDocument = new XDocument();

                XComment warning = new XComment("Warning, these are dummy Impls! You might be able to use them but it's not recommended!");
                XComment warning2 = new XComment("Inheritance is resolved at runtime, therefore you won't find parent tags!");
                XComment warning3 = new XComment("References and various other complex datatypes won't be serialized correctly!");
                XElement root = new XElement("ImplDump");

                loadImplDocument.Add(warning, warning2, warning3, root);

                foreach (var package in CargoBay.LoadedPackages.Values)
                {
                    XElement packageRoot = new XElement(package.id);
                    root.Add(packageRoot);

                    foreach (var implPair in package.packageImpls)
                    {
                        XElement loadedNode = new XElement("Impl");
                        loadedNode.Add(new XAttribute("Class", implPair.Value.GetType()));

                        foreach (var field in implPair.Value.GetType().GetFields())
                        {
                            // TODO: More complex serialization?
                            XElement fieldNode = new XElement(field.Name, field.GetValue(implPair.Value));
                            loadedNode.Add(fieldNode);
                        }

                        packageRoot.Add(loadedNode);
                    }
                }

                loadImplDocument.Save(CargoBayDebugPath + "/LoadedImpls.xml");
            }

            {
                XDocument typeDocument = new XDocument();

                XComment notice = new XComment("This is a readout of all loaded and cached Impl types! Inheritance is printed but might be formatted incorrectly!");
                XElement root = new XElement("Types");

                XElement implTypes = new XElement("ImplTypes");
                XElement implSubtypes = new XElement("ImplSubdataTypes");

                typeDocument.Add(notice, root);
                root.Add(implTypes, implSubtypes);

                foreach (var type in Impl.RegisteredImplTypes)
                {
                    XComment fullName = new XComment(type.FullName);
                    XElement typeNode = new XElement(type.Name);

                    if (type.BaseType != null)
                    {
                        XComment fullParentName = new XComment($"FullParent: {type.BaseType.FullName}");
                        XElement parentNode = new XElement("Parent", type.BaseType.Name);
                        typeNode.Add(fullParentName, parentNode);
                    }

                    implTypes.Add(fullName, typeNode);
                }

                foreach (var type in Impl.RegisteredImplDataTypes)
                {
                    XComment fullName = new XComment(type.FullName);
                    XElement typeNode = new XElement(type.Name);

                    if (type.BaseType != null)
                    {
                        XComment fullParentName = new XComment($"FullParent: {type.BaseType.FullName}");
                        XElement parentNode = new XElement("Parent", type.BaseType.Name);
                        typeNode.Add(fullParentName, parentNode);
                    }

                    if (type.IsNested)
                    {
                        XComment fullParentName = new XComment($"FullChildOf: {type.DeclaringType.FullName}");
                        XElement parentNode = new XElement("ChildOf", type.DeclaringType.Name);
                        typeNode.Add(fullParentName, parentNode);
                    }

                    implSubtypes.Add(typeNode);
                }

                typeDocument.Save(CargoBayDebugPath + "/ImplTypeInfo.xml");
            }
        }
#endif
    }
}
