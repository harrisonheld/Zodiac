using System;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

public static class Serialization
{
    public static void SerailizeToFile(GameObject obj, string path)
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };
        XmlWriter writer = XmlWriter.Create(path, settings);
        writer.WriteStartElement("root");

        foreach(ZodiacComponent comp in obj.GetComponents<ZodiacComponent>())
        {
            Type compType = comp.GetType();
            writer.WriteStartElement(compType.Name);

            // get all public instance (ie, non static) fields
            PropertyInfo[] propertyInfos = compType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    var fieldName = propertyInfo.Name;
                    object fieldValue = propertyInfo.GetValue(comp);
                    writer.WriteAttributeString(fieldName, fieldValue.ToString());
                }
                catch
                {
                    Debug.Log($"Cannot serailize property {propertyInfo.Name} of type {propertyInfo.PropertyType} for component type {compType.Name}");
                }
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.Close();
    }

    public static GameObject Deserialize(string path)
    {
        GameObject entity = new();
        
        XmlTextReader reader = new(path);
        while(reader.Read())
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element: // The node is an element.
                    Console.Write("<" + reader.Name);
                    Console.WriteLine(">");
                    break;

                case XmlNodeType.Text: //Display the text in each element.
                    Console.WriteLine(reader.Value);
                    break;

                case XmlNodeType.EndElement: //Display the end of the element.
                    Console.Write("</" + reader.Name);
                    Console.WriteLine(">");
                    break;
            }
        }

        return entity;
    }
}
