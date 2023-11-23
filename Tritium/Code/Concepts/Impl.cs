using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

using Tritium.Logging;
using Tritium.Assets;

namespace Tritium.Concepts
{
    // Inspired by RimWorld

    // TODO: We need internal fallbacks for classes!

    /// <summary>
    /// An "Impl" or "Implementation" is an XML defined data structure
    /// It's loaded from XML documents and used to create instances of entities and various data types
    /// </summary>
    public abstract class Impl
    {
        /// <summary>
        /// Whitelists a type to be cached in the Impl loader
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class SubDataAttribute : Attribute { }

        // Subtypes are anonymous, they're read automatically by the XML handler
        public class ImplRef : GenericRef<Impl>
        {
            public ImplRef() : base() { }
            public ImplRef(string softRef) : base(softRef) { }

            protected override Impl FetchReference()
            {
                ResolveContainer();

                // For references, we trim padding slashes
                string realRef = softRef.TrimStart('/');

                return container.packageImpls[realRef];
            }

            public static implicit operator ImplRef(string softRef) => new ImplRef(softRef);
        }

        public class StrictImplRef<T> : ImplRef where T : Impl
        {
            public StrictImplRef() : base() { }
            public StrictImplRef(string softRef) : base(softRef) { }

            public static implicit operator T(StrictImplRef<T> r) => r.Resolve() as T;
            public static implicit operator StrictImplRef<T>(string softRef) => new StrictImplRef<T>(softRef);
        }

        //
        // C# Frontend
        //
        public static bool AutomatedRegister = false;

        public class ImplCreationScope : IDisposable
        {
            public ImplCreationScope()
            {
                AutomatedRegister = true;
            }

            public void Dispose()
            {
                AutomatedRegister = false;
            }
        }

        //
        // Members
        //
        public static string InvalidImplName { get => "UnknownImpl"; }

        public string implName = InvalidImplName;
        public bool isAbstract = false; // Abstract Impl's are not allowed to be instantiated

        //
        // Statics
        //
        /// <summary>
        /// The expected root element name for an Impls schema document
        /// </summary>
        public static XName ImplRootName => "Impls";

        /// <summary>
        /// A cache of all known classes deriving from Impl
        /// </summary>
        public static List<Type> RegisteredImplTypes { get; protected set; } = null;

        /// <summary>
        /// A cache of all whitelisted subdata classes used by Impl (whitelist using ImplSubDataAttribute)
        /// </summary>
        public static List<Type> RegisteredImplDataTypes { get; protected set; } = null;

        /// <summary>
        /// Sets up the internal registry for impl types
        /// Since mods can define their own Impl types, we must register the types after loading mods!
        /// MUST BE CALLED AFTER MOD ASSEMBLIES ARE LOADED!
        /// </summary>
        protected static void RegisterImplTypes()
        {
            RegisteredImplTypes = new List<Type>();
            RegisteredImplDataTypes = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var implFilter = assembly.GetTypes().Where((type) => typeof(Impl).IsAssignableFrom(type));
                RegisteredImplTypes.AddRange(implFilter);

                var subTypeFilter = assembly.GetTypes().Where(type => type.GetCustomAttribute<SubDataAttribute>() != null);
                RegisteredImplDataTypes.AddRange(subTypeFilter);

#if DEBUG
                implFilter.ForEach((type) => TritiumGame.Logger.Log($"Registered: {type}"));
#endif
            }
        }

        public static Type GetImplType(string typeName) => RegisteredImplTypes.FirstOrDefault((type) => type.Name == typeName || type.FullName == typeName);
        public static Type GetImplType(XName typeName) => GetImplType(typeName.ToString());

        // Attempts to read an XML doc and register the types
        public static void RegisterXDocument(CargoPackage package, XDocument document)
        {
            if (RegisteredImplTypes == null)
                RegisterImplTypes();

            if (document?.Root?.Name == ImplRootName)
                foreach (var elem in document?.Root.Elements())
                    RegisterXElement(package, elem);
        }

        /// <summary>
        /// Attempts to register an XElement inside the Impl database
        /// </summary>
        /// <param name="elem">Target element</param>
        /// <param name="parentImpl">The Impl that the element inherits from</param>
        /// <exception cref="Exception">Thrown when failing to load</exception>
        public static void RegisterXElement(CargoPackage package, XElement elem, Impl parentImpl = null)
        {
            elem.TryGetAttribute("Class", out string xmlTypeName, elem.Name.ToString());

            // We need to verify this element is referencing a valid type
            Type implType = GetImplType(xmlTypeName);

            if (implType != null)
            {
#if DEBUG
                TritiumGame.Logger.Log($"Found Type in XML: {xmlTypeName}");
#endif



                // TODO: Can overriding be implicit?
                elem.TryParseBoolAttribute("Override", out bool ohverride);

                // We then instantiate an instance of this type
                // This is for both inheritance but also overriding
                Impl impl = parentImpl;

                if (elem.TryGetAttribute("Parent", out string parent))
                {
                    if (!package.packageImpls.ContainsKey(parent))
                        throw new Exception($"Failed to find impl called '{parent}' to inherit from!");
                    else
                        impl = package.packageImpls[parent];
                }

                if (impl == null)
                    impl = Activator.CreateInstance(implType) as Impl;
                else
                    impl = CargoXScribe.DeepCopy(impl, implType) as Impl;

                if (impl != null)
                {
                    // We then need to setup this Impl
                    // Note: Impl's are not actually used by the game, they're instead copied into ImplEntity instances!
                    // Note: This is because Impl's would be too complex to duplicate, so instead they're stored as recipes for entities
                    var obj = (object)impl;
                    CargoXScribe.PopulateTypeFields(elem, implType, ref obj);
                }

                if (impl.implName != InvalidImplName)
                {
                    // TODO: Enforce override being defined beforehand?
                    // Impls are unique to packages
                    if (ohverride && package.packageImpls.ContainsKey(impl.implName))
                        package.packageImpls[impl.implName] = impl;
                    else
                        package.packageImpls.Add(impl.implName, impl);

                    // Check if this element has nested types
                    // TODO: Optimization?
                    foreach (var subElem in elem.Elements())
                        RegisterXElement(package, subElem, impl);
                }
                else
                    throw new Exception("Failed to register Impl, key was already taken or key was invalid!");
            }
        }

        //
        // Base constructors
        //
        public Impl() { }
        public Impl(string implName, bool isAbstract)
        {
            this.implName = implName;
            this.isAbstract = isAbstract;
        }
    }
}
