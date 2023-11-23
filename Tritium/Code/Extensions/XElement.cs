using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace Tritium
{
    public static class XmlElementExtensions
    {
        public static bool TryGetAttribute(this XElement element, string attribute, out string value, string onNullData = "", bool caseInsensitive = true)
        {
            XAttribute xattribute = null;
            
            if (xattribute == null)
                xattribute = element.Attributes().FirstOrDefault(x => string.Equals(x.Name.LocalName, attribute, StringComparison.OrdinalIgnoreCase));
            else
                xattribute = element.Attribute(attribute);

            value = xattribute != null ? xattribute.Value : onNullData;
            return xattribute != null;
        }

        public static bool TryParseIntAttribute(this XElement element, string attribute, out int value, int onNullData = 0, bool caseInsensitive = true)
        {
            value = onNullData;

            if (TryGetAttribute(element, attribute, out string rawValue, "", caseInsensitive))
                return int.TryParse(rawValue, out value);

            return false;
        }

        public static bool TryParseFloatAttribute(this XElement element, string attribute, out float value, float onNullData = 0.0f, bool caseInsensitive = true)
        {
            value = onNullData;

            if (TryGetAttribute(element, attribute, out string rawValue, "", caseInsensitive))
                return float.TryParse(rawValue, out value);

            return false;
        }

        public static bool TryParseBoolAttribute(this XElement element, string attribute, out bool value, bool onNullData = false, bool caseInsensitive = true)
        {
            value = onNullData;

            if (TryGetAttribute(element, attribute, out string rawValue, "", caseInsensitive))
                return bool.TryParse(rawValue, out value);

            return false;
        }

        public static string GetInnerText(this XElement element)
        {
            return element?.Nodes().FirstOrDefault(node => node.NodeType == System.Xml.XmlNodeType.Text)?.ToString().Trim('\n', '\r', ' ');
        }
    }
}
