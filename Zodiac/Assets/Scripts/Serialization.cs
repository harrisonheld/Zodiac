using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

public static class Serialization 
{
    const string ENTITIES = "Entities"; // the start marker for all entities
    const string ENTITY = "Entity"; // the start marker for an entity
    
    public static void SerializeEntities(IEnumerable entities, string path)
    {
        // create a writer
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true,
        };
        using XmlWriter writer = XmlWriter.Create(path, settings);

        writer.WriteStartElement(ENTITIES);
            foreach (GameObject entity in entities)
            {
                SerializeEntityToWriter(writer, entity);
            }
        writer.WriteEndElement();
    }
    public static void SerializeEntity(GameObject entity, string path)
    {
        // create a writer
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };
        using XmlWriter writer = XmlWriter.Create(path, settings);
        
        SerializeEntityToWriter(writer, entity);
    }
    
    
    
    private static void SerializeEntityToWriter(XmlWriter writer, GameObject entity)
    {
        writer.WriteStartElement(ENTITY);

        //foreach (ZodiacComponent comp in entity.GetComponents<ZodiacComponent>())
            //comp.Serialize(writer);

        writer.WriteEndElement();
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

                case XmlNodeType.Text:
                    // currently i don't really use text in the serialization
                    break;

                case XmlNodeType.EndElement:
                    // who care
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