using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Xml;
using System.IO;

/// <summary>
/// Base class for other components to inherit from
/// </summary>
public abstract class ZodiacComponent : MonoBehaviour
{
    public virtual List<IInteraction> GetInteractions() { return new(); }
    public virtual string GetDescription() { return null; }



    public virtual void Serialize(BinaryWriter writer)
    {
        // get all properties
        var properties = GetProperties();
        foreach (var property in properties)
        {
            // check if the property has the NoSerialize attribute
            if (property.GetCustomAttribute(typeof(ZodiacNoSerializeAttribute)) != null)
                continue;

            // no need to write the property name, we will just serialize and deserialize in the same order order
            var value = property.GetValue(this);
            bool hasValue = value != null;
            // write a bool indicating if we have this property
            writer.Write(hasValue);
            // then write the property
            if (hasValue)
            {
                writer.Write(value.ToString());
            }
        }
    }
    public virtual void Deserialize(BinaryReader reader, Dictionary<int, GameObject> idToEntity)
    {
        // deserialize into this component
        var properties = GetProperties();
        foreach (var property in properties)
        {
            // check if the property has the NoSerialize attribute
            if (property.GetCustomAttribute(typeof(ZodiacNoSerializeAttribute)) != null)
                continue;

            // read the property value
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                string value = reader.ReadString();
                object converted = ConvertToType(value, property.PropertyType);
                property.SetValue(this, converted);
            }
        }
    }
    private PropertyInfo[] GetProperties() => GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

    protected object ConvertToType(string value, Type type)
    {
        if(type.IsEnum)
        {
            return Enum.Parse(type, value);
        }

        return Convert.ChangeType(value, type);

        throw new NotImplementedException($"Cannot convert '{value}' to type '{type}'.");
    }

    protected void WriteColor(BinaryWriter writer, Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
    }
    protected Color ReadColor(BinaryReader reader)
    {
        float r = reader.ReadSingle();
        float g = reader.ReadSingle();
        float b = reader.ReadSingle();
        return new Color(r, g, b);
    }

    protected void WriteEntity(BinaryWriter writer, GameObject entity)
    {
        if(entity == null)
        {
            writer.Write(0);
            return;
        }

        writer.Write(entity.GetInstanceID());
    }
    protected GameObject ReadEntity(BinaryReader reader, Dictionary<int, GameObject> idToEntity)
    {
        int id = reader.ReadInt32();

        if (id == 0)
            return null;

        if (!idToEntity.ContainsKey(id))
            idToEntity.Add(id, new GameObject());

        return idToEntity[id];
    }

    protected void WriteEntityList(BinaryWriter writer, List<GameObject> entities)
    {
        writer.Write(entities.Count);
        foreach (var entity in entities)
        {
            WriteEntity(writer, entity);
        }
    }
    protected List<GameObject> ReadEntityList(BinaryReader reader, Dictionary<int, GameObject> idToEntity)
    {
        int count = reader.ReadInt32();

        List<GameObject> entities = new();
        for(int i = 0; i < count; i++)
        {
            GameObject entity = ReadEntity(reader, idToEntity);
            entities.Add(entity);
        }

        return entities;
    }



    public virtual void HandleEvent(ZodiacEvent e)
    {
        // this will call the appropriate HandleEvent for the event type
        dynamic casted = e;
        HandleEvent(casted);
    }
    public virtual bool HandleEvent(PickedUpEvent e) { return false; }
    public virtual bool HandleEvent(LookedAtEvent e) { return false; }
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ZodiacNoSerializeAttribute : System.Attribute
{

}