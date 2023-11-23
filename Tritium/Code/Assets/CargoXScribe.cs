using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

using Microsoft.Xna.Framework;


using Tritium.Logging;
using Tritium.Concepts;

namespace Tritium.Assets
{
    /// <summary>
    /// Used by classes like "CargoBay" to load Xml data through reflection and XML Linq
    /// </summary>
    public static class CargoXScribe
    {
        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
        public class CargoXLoaderAttribute : Attribute
        {
            public Type readType = null;

            public CargoXLoaderAttribute(Type type)
            {
                readType = type;
            }
        }

        public static class BuiltInXReadFuncs
        {
            [CargoXLoader(typeof(Vector2))]
            public static object XmlReadVector2(string data)
            {
                var v2 = new Vector2();
                v2.Parse(data);
                return v2;
            }

            [CargoXLoader(typeof(Vector3))]
            public static object XmlReadVector3(string data)
            {
                var v3 = new Vector3();
                v3.Parse(data);
                return v3;
            }

            [CargoXLoader(typeof(Rectangle))]
            public static object XmlReadRectangle(string data)
            {
                var rect = new Rectangle();
                rect.Parse(data);
                return rect;
            }

            [CargoXLoader(typeof(Color))]
            public static object XmlReadColor(string data)
            {
                var col = new Color();
                col.LoadHex(data);
                return col;
            }
        }

        public delegate object XReadFunction(string str);

        public static readonly Dictionary<Type, XReadFunction> XReadFunctions = new Dictionary<Type, XReadFunction>();
        private static bool HasSetupRead = false;

        public static void RegisterXReadFunctions()
        {
            MethodInfo readFuncInvoke = typeof(XReadFunction).GetMethod("Invoke");
            Type expectedReturn = readFuncInvoke.ReturnType;
            ParameterInfo[] expectedParams = readFuncInvoke.GetParameters();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    var filterMethods = t.GetMethods().Where((m) => m.GetCustomAttribute<CargoXLoaderAttribute>() != null);

                    // Verify the methods match the delegate signature
                    foreach (MethodInfo m in filterMethods)
                    {
                        var attribute = m.GetCustomAttribute<CargoXLoaderAttribute>();

                        if (!m.IsStatic && !m.IsPublic)
                            throw new Exception($"XReadFunc requires a function to be static and public! Error for method {m}");

                        if (m.ReturnType != expectedReturn)
                            throw new Exception($"Mismatched return type, expected {expectedReturn}, got {m.ReturnType}!");

                        ParameterInfo[] parameters = m.GetParameters();

                        if (parameters.Length != expectedParams.Length)
                            throw new Exception($"Mismatched parameter length! Expected {expectedParams.Length}, got {parameters.Length}");

                        for (int p = 0; p < parameters.Length; p++)
                            if (parameters[p].ParameterType != expectedParams[p].ParameterType)
                                throw new Exception($"Mismatched parameter type! Expected {expectedParams[p].ParameterType}, got {parameters[p].ParameterType}");

                        XReadFunction func = m.CreateDelegate<XReadFunction>();
                        XReadFunctions.Add(attribute.readType, func);
                    }
                }
            }

            HasSetupRead = true;
        }

        // TODO: Make Impl specific stuff self-contained?
        public static void PopulateTypeFields(XElement root, Type type, ref object obj)
        {
            if (!HasSetupRead)
                RegisterXReadFunctions();

            // TODO: Properties?
            bool isImpl = typeof(Impl).IsAssignableFrom(type);

            if (isImpl)
            {
                Impl impl = obj as Impl;

                root.TryParseBoolAttribute("Abstract", out impl.isAbstract, false);
                root.TryGetAttribute("Name", out impl.implName, "");
            }

            foreach (var memberElem in root.Elements())
            {
                var fieldInfo = type.GetField(memberElem.Name.ToString());

                if (fieldInfo != null)
                {
#if DEBUG
                    TritiumGame.Logger.Log($"Found Field in XML: {fieldInfo}");
#endif

                    // We need to get the contained data first since it has many usages
                    string rawData = memberElem.GetInnerText();

                    Type listElementType = null;
                    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        listElementType = fieldInfo.FieldType.GenericTypeArguments[0];

                    // Things get strange if we're writing to a list
                    // We need to write to a list, so we call this method again
                    // But we need to strip each instance of <li></li>
                    if (listElementType != null)
                    {
                        object listObject = Activator.CreateInstance(fieldInfo.FieldType);
                        var addMethod = fieldInfo.FieldType.GetMethod("Add");

                        foreach (var listElem in memberElem.Elements())
                        {
                            // If li isn't a reference we populate it, otherwise we have to assign the reference
                            object listBuffer = null;
                            if (typeof(Impl.ImplRef).IsAssignableFrom(listElementType))
                            {
                                Impl.ImplRef implRef = Activator.CreateInstance(listElementType) as Impl.ImplRef;
                                listBuffer = implRef;

                                implRef.softRef = listElem.GetInnerText();
                            }
                            else if (listElementType == typeof(string))
                            {
                                listBuffer = listElem.GetInnerText();
                            }
                            else
                            {
                                listBuffer = Activator.CreateInstance(listElementType);
                                PopulateTypeFields(listElem, listElementType, ref listBuffer);
                            }

                            addMethod.Invoke(listObject, new object[] { listBuffer });
                        }

                        fieldInfo.SetValue(obj, listObject);
                        continue;
                    }

                    Type compareType = listElementType == null ? fieldInfo.FieldType : listElementType;

                    // Because generics are compiler magic, we need to "traverse the type heirarchy"
                    bool isRef = false;

                    Type baseType = compareType.BaseType;
                    while (baseType != null && !isRef)
                    {
                        if (baseType.IsGenericType)
                            isRef = baseType.GetGenericTypeDefinition() == typeof(GenericRef<>);

                        baseType = baseType.BaseType;
                    }

                    if (isRef)
                    {
                        Type refType = null;
                        if (compareType.IsGenericType)
                        {
                            Type[] types = compareType.GenericTypeArguments;
                            Type genericDef = compareType.GetGenericTypeDefinition();
                            refType = genericDef.MakeGenericType(compareType.GenericTypeArguments);
                        }
                        else
                            refType = compareType;

                        object contentRef = Activator.CreateInstance(refType);
                        compareType.GetMethod("ChangePath").Invoke(contentRef, new object[] { rawData });

                        fieldInfo.SetValue(obj, contentRef);
                        continue;
                    }

                    // Subdata is implicit
                    if (Impl.RegisteredImplDataTypes != null)
                    {
                        Type subDataType = Impl.RegisteredImplDataTypes.FirstOrDefault(type => type == compareType);
                        if (subDataType != null)
                        {
                            object subData = Activator.CreateInstance(subDataType);
                            PopulateTypeFields(memberElem, subDataType, ref subData);

                            fieldInfo.SetValue(obj, subData);
                            continue;
                        }
                    }

                    if (XReadFunctions.ContainsKey(fieldInfo.FieldType))
                    {
                        try
                        {
                            object data = XReadFunctions[fieldInfo.FieldType](rawData);
                            fieldInfo.SetValue(obj, data);
                            continue;
                        } 
                        catch
                        {
                            // TODO: Catch this?
                        }
                    }

                    if (typeof(Enum).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        if (Enum.TryParse(fieldInfo.FieldType, rawData, true, out object inum))
                            fieldInfo.SetValue(obj, inum);

                        continue;
                    }

                    // If we don't find any numbers, it's assumed to be a bool
                    bool isNumericalType = fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(float);

                    if (isNumericalType)
                    {
                        if (float.TryParse(rawData, out float floatData))
                        {
                            if (fieldInfo.FieldType == typeof(float))
                                fieldInfo.SetValue(obj, floatData);
                            else
                                fieldInfo.SetValue(obj, (int)floatData);
                        }
                        else
                            throw new Exception("Can't set a numerical field to a string value!");

                        continue;
                    }

                    // If we don't find a bool, it's just a string
                    if (fieldInfo.FieldType == typeof(bool))
                    {
                        if (bool.TryParse(rawData, out bool boolData))
                            fieldInfo.SetValue(obj, boolData);
                        else
                            throw new Exception("Can't set a boolean field to a string value!");

                        continue;
                    }

                    fieldInfo.SetValue(obj, rawData);
                }
                else
                {
                    // Is this an Impl type?
                    if (Impl.RegisteredImplTypes != null)
                        if (Impl.GetImplType(memberElem.Name) == null)
                            throw new Exception($"Failed to find field '{memberElem.Name}' in type {type}! Did you specify the wrong type?");
                }
            }
        }

        public static void PopulateTypeFields<T>(XElement root, ref T instance)
        {
            object obj = (object)instance;
            PopulateTypeFields(root, typeof(T), ref obj);
        }

        public static object DeepCopy(object original, Type newType)
        {
            // We need to make sure the original is at least assignable to the other
            if (!newType.IsAssignableTo(original.GetType()))
                throw new Exception($"DeepCopy can only inherit from a parent type! Tried to inherit {original.GetType()} -> {newType}");
            else
            {
                object copy = Activator.CreateInstance(newType);

                foreach (var field in original.GetType().GetFields())
                    if (!field.IsInitOnly && !field.IsStatic && !field.IsLiteral)
                        field.SetValue(copy, field.GetValue(original));

                return copy;
            }
        }

        public static T DeepCopy<T, TNew>(T original) where T: class => DeepCopy(original, typeof(TNew)) as T;
    }
}
