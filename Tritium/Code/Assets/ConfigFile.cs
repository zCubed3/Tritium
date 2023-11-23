using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Tritium.Assets
{
    /// <summary>
    /// Provides a simple solution for easily adding config files, made with modding in mind
    /// </summary>
    public abstract class ConfigFile
    {
        /// <summary>
        /// The name of this config file (Extension is appended when written)
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The file extension for this config file
        /// </summary>
        public virtual string Extension => ".xml";

        /// <summary>
        /// The full name (name + extension)
        /// </summary>
        public virtual string FullName => $"{Name}{Extension}";

        // TODO: Let this be changed?
        /// <summary>
        /// The config folder relative to the game executable
        /// </summary>
        public static string ConfigFolder => "Config/";

        /// <summary>
        /// The full path of this config file
        /// </summary>
        public virtual string FullPath => $"{ConfigFolder}{FullName}";

        protected virtual bool ValidateFile() => File.Exists(FullPath);

        /// <summary>
        /// Loads an instance of a config file based on the provided generic type
        /// </summary>
        /// <typeparam name="T">A ConfigFile subtype</typeparam>
        /// <returns>An instance of T, otherwise null if there's been an error</returns>
        public static T Load<T>() where T: ConfigFile, new()
        {
            T instance = new T();

            if (instance.ValidateFile())
            {
                try
                {
                    XDocument document = XDocument.Load(instance.FullPath);
                    CargoXScribe.PopulateTypeFields<T>(document.Root, ref instance);
                }
                catch (Exception e)
                {
                    TritiumGame.Logger.Log($"Encountered exception while loading {typeof(T)}!\n{e}", Logging.LoggerInstance.LogSeverity.Error);
                    return null; // Error, therefore we need to prove there was one
                }
            }
            else
                instance.Save(); // Ensures we have saved instance

            return instance;
        }

        /// <summary>
        /// Ensures the path we're writing to exists
        /// </summary>
        protected virtual void EnsurePath()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);
        }

        /// <summary>
        /// Writes the provided text to the output file
        /// </summary>
        /// <param name="text">Text to be written</param>
        protected virtual void WriteText(string text) 
        {
            EnsurePath();
            File.WriteAllText(FullPath, text);
        }

        /// <summary>
        /// Opens the output file as a stream
        /// </summary>
        /// <returns>Output file stream</returns>
        protected virtual FileStream OpenFile()
        {
            EnsurePath();
            return File.Open(FullPath, File.Exists(FullPath) ? FileMode.Truncate : FileMode.Create);
        }

        /// <summary>
        /// Call to overwrite or create the config file, by default serializes to XML using .NET's default XML serializer
        /// </summary>
        public virtual void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (FileStream fs = OpenFile())
            using (XmlWriter writer = XmlWriter.Create(fs, settings))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
            }
        }
    }
}
