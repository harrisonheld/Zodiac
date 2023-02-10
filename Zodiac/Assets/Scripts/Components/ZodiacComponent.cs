using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Xml;

/// <summary>
/// Base class for other components to inherit from
/// </summary>
public abstract class ZodiacComponent : MonoBehaviour
{
    public virtual List<IInteraction> GetInteractions() { return new(); }
    public virtual string GetDescription() { return null; }

    
    public virtual void Serialize(EntitySerializer2 writer)
    {
        Type type = this.GetType();
        writer.WriteStartElement(type.Name);

        // get all public instance (ie, non static) fields
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var propertyInfo in propertyInfos)
        {
            // check if the property has the ZodiacNoSerialize attribute
            if (Attribute.IsDefined(propertyInfo, typeof(ZodiacNoSerializeAttribute)))
                continue;

            var propName = propertyInfo.Name;
            object propValue = propertyInfo.GetValue(this);

            writer.WriteProperty(propName, propValue);
        }

        writer.WriteEndElement();
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