using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class EntitySerializer
{
    private Dictionary<GameObject, int> ObjectIds = new();
    private Dictionary<ZodiacComponent, int> ComponentIds = new();
    private int entityCount = 0; // count entites to assign id's incrementally
    private int componentCount = 0; // count components to assign id's incrementally

    private Dictionary<int, GameObject> EntityIdToGameObject = new();
    private Dictionary<int, ZodiacComponent> ComponentIdToComponent = new();

    private TextWriter writer;
    private JsonSerializer serializer;

    public void SerializeScene(string path)
    {
        Clear();

        serializer = new JsonSerializer();
        writer = new StreamWriter(path);


        // serialize all components
        List<string> componentJsons = new List<string>();
        foreach (GameObject entity in GameManager.Instance.Entities)
        {
            foreach (ZodiacComponent component in entity.GetComponents<ZodiacComponent>())
            {
                SerializeComponentProperties(component);
            }
        }
        string componentListJson = JsonUtility.ToJson(componentJsons);
    }
    public void SerializeComponentProperties(ZodiacComponent component)
    {
        Type componentType = component.GetType();

        SerializedComponentData data = new SerializedComponentData();
        data.ComponentType = componentType;
        data.ComponentId = GetComponentId(component);

        PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (PropertyInfo property in properties)
        {
            if (!property.GetCustomAttributes(typeof(ZodiacNoSerializeAttribute), true).Any())
            {
                data.AddProperty(property.Name, property.GetValue(component));
            }
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Converters.Add(new ColorConverter());
        string json = JsonConvert.SerializeObject(data, settings);
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


    public int GetEntityId(GameObject entity)
    {
        // if this entity has yet to be referenced, assign it a new id
        if (!ObjectIds.ContainsKey(entity))
            ObjectIds.Add(entity, entityCount++);

        return ObjectIds[entity];
    }
    public int GetComponentId(ZodiacComponent component)
    {
        // if this entity has yet to be referenced, assign it a new id
        if (!ComponentIds.ContainsKey(component))
            ComponentIds.Add(component, componentCount++);

        return ComponentIds[component];
    }

    [Serializable]
    private class SerializedComponentData
    {
        public Type ComponentType;
        public int ComponentId;
        public Dictionary<string, object> Properties = new Dictionary<string, object>();

        public void AddProperty(string name, object value)
        {
            Properties[name] = value;
        }
    }

    private class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string colorString = reader.Value.ToString();
            float[] rgb = colorString.Split(',').Select(component => float.Parse(component)).ToArray();
            return new Color(rgb[0], rgb[1], rgb[2]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color color = (Color)value;
            string colorString = $"{color.r},{color.g},{color.b}";
            writer.WriteValue(colorString);
        }
    }
}


[System.AttributeUsage(System.AttributeTargets.Property)]
public class ZodiacNoSerializeAttribute : System.Attribute
{
}