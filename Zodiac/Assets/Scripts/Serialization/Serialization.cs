using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;

public class EntitySerializer
{
    private Dictionary<GameObject, int> ObjectIds = new();
    private Dictionary<ZodiacComponent, int> ComponentIds = new();
    private int entityCount = 0; // count entites to assign id's incrementally
    private int componentCount = 0; // count components to assign id's incrementally

    private Dictionary<int, GameObject> EntityIdToGameObject = new();
    private Dictionary<int, ZodiacComponent> ComponentIdToComponent = new();

    public void SerializeScene(string path)
    {
        Clear();

        // serialize all components
        List<string> componentJsons = new List<string>();
        foreach (GameObject entity in GameManager.Instance.Entities)
        {
            foreach (ZodiacComponent component in entity.GetComponents<ZodiacComponent>())
            {
                string json = JsonUtility.ToJson(component);
                componentJsons.Add(json);
            }
        }
        string componentListJson = JsonUtility.ToJson(componentJsons);
    }
    public List<GameObject> DeserializeScene(string path)
    {
        Clear();
        return null;
    }

    private void Clear()
    {
        ObjectIds.Clear();
        entityCount = 0;
        ComponentIds.Clear();
        componentCount = 0;

        EntityIdToGameObject.Clear();
        ComponentIdToComponent.Clear();
    }

    private string ComponentToJson(ZodiacComponent component)
    {
        Dictionary<string, object> serializedData = new Dictionary<string, object>();
        serializedData.Add("ComponentId", GetComponentId(component));

        foreach (var propertyType in component.GetType().GetProperties())
        {
            if (propertyType.PropertyType == typeof(GameObject))
            {
                GameObject gameObjectRef = (GameObject)propertyType.GetValue(component);
                serializedData[propertyType.Name] = GetEntityId(gameObjectRef);
            }
            else if (typeof(ZodiacComponent).IsAssignableFrom(propertyType.PropertyType))
            {
                ZodiacComponent zodiacComponentRef = (ZodiacComponent)propertyType.GetValue(component);
                serializedData[propertyType.Name] = GetComponentId(zodiacComponentRef);
            }
            else
            {
                serializedData[propertyType.Name] = propertyType.GetValue(component);
            }
        }

        return JsonUtility.ToJson(serializedData);
    }


    private int GetEntityId(GameObject entity)
    {
        // if this entity has yet to be referenced, assign it a new id
        if (!ObjectIds.ContainsKey(entity))
            ObjectIds.Add(entity, entityCount++);

        return ObjectIds[entity];
    }
    private int GetComponentId(ZodiacComponent component)
    {
        // if this entity has yet to be referenced, assign it a new id
        if (!ComponentIds.ContainsKey(component))
            ComponentIds.Add(component, componentCount++);

        return ComponentIds[component];
    }
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ZodiacNoSerializeAttribute : System.Attribute
{
}