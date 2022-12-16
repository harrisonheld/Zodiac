using System;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

public static class Serialization
{
    const string ENTITY = "Entity"; // the start marker for an entity
    public static void SerializeToFile(GameObject obj, string path)
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };
        XmlWriter writer = XmlWriter.Create(path, settings);
        writer.WriteStartElement(ENTITY);

        foreach(ZodiacComponent comp in obj.GetComponents<ZodiacComponent>())
        {
            Type compType = comp.GetType();
            writer.WriteStartElement(compType.Name);

            // get all public instance (ie, non static) fields
            PropertyInfo[] propertyInfos = compType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var propertyInfo in propertyInfos)
            {
                // check if the property has the ZodiacNoSerialize attribute
                if (Attribute.IsDefined(propertyInfo, typeof(ZodiacNoSerializeAttribute)))
                    continue;
                
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
        entity.AddComponent<SpriteRenderer>();

        using XmlTextReader reader = new(path);
        while(reader.Read())
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element: // The node is an element.
                    if (reader.Name == ENTITY)
                        continue;
                    // create the component
                    Type compType = Type.GetType(reader.Name);
                    ZodiacComponent comp = (ZodiacComponent)entity.AddComponent(compType);
                    // set props
                    for(int i = 0; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToAttribute(i);
                        string propertyName = reader.Name;
                        string propertyValueString = reader.Value;
                        
                        try
                        {
                            PropertyInfo prop = compType.GetProperty(propertyName);

                            // convert the string to the correct type, including if the type is an enum
                            if (prop.PropertyType.IsEnum)
                                prop.SetValue(comp, Enum.Parse(prop.PropertyType, propertyValueString));
                            else
                                prop.SetValue(comp, Convert.ChangeType(propertyValueString, prop.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.Message);
                        }

                    }

                    break;

                case XmlNodeType.Text: //Display the text in each element.
                    Debug.Log(reader.Value);
                    break;

                case XmlNodeType.EndElement: //Display the end of the element.
                    Debug.Log("<" + reader.Name + ">");
                    break;
            }
        }

        return entity;
    }
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ZodiacNoSerializeAttribute : System.Attribute
{
}