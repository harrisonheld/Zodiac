using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using System.Linq;

public class EntitySerializer2
{
    private const string ENTITIES = "Entities"; // the start marker for all entities
    private const string ENTITY = "Entity"; // the start marker for an entity
    private const string ENTITY_ID = "ID"; // used to identify objects. entity references will use this
    private const string ENTITY_COUNT = "EntityCount"; // used to say how many entities are in the doc
    private const string COMPONENT = "Component";
    private const string COMPONENT_TYPE = "Type";
    private const string PROPERTY = "Property";
    private const string PROPERTY_NAME = "PropertyName";
    private const string PROPERTY_VALUE = "PropertyValue";

    private readonly XmlWriterSettings writerSettings = new()
    {
        Indent = true,
        IndentChars = "\t",
    };
    private readonly XmlReaderSettings readerSettings = new()
    {

    };
    private XmlWriter writer;
    private XmlReader reader;
    
    private Dictionary<GameObject, int> ObjectIds = new();
    private int entityCount = 0; // count entites to assign id's incrementally

    public void SerializeScene(List<GameObject> toSerialize, string path)
    {
        Clear();
        
        writer = XmlWriter.Create(path, writerSettings);
        writer.WriteStartElement(ENTITIES);
        writer.WriteAttributeString(ENTITY_COUNT, toSerialize.Count.ToString());

        foreach (GameObject entity in toSerialize)
        {
            WriteEntity(entity);
        }

        writer.WriteEndElement();
        writer.Close();
        writer.Dispose();
    }
    public GameObject[] DeserializeScene(string path)
    {
        Clear();
        
        reader = XmlReader.Create(inputUri:path, settings:readerSettings);

        // array of entities where each entity is indexed at its ID
        GameObject[] entities = null;
        
        GameObject workingEntity = null;
        ZodiacComponent workingComponent = null;

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        if (reader.Name == ENTITIES)
                        {
                            // so we first have to create all the entities and assign them IDs so they will 
                            // exist when it comes time to write the references in
                            int toCreate = int.Parse(reader.GetAttribute(ENTITY_COUNT));
                            entities = new GameObject[toCreate];

                            for (int i = 0; i < toCreate; i++)
                            {
                                entities[i] = new GameObject();
                                ObjectIds[entities[i]] = i;
                            }
                        }
                        else if (reader.Name == ENTITY)
                        {
                            int id = int.Parse(reader.GetAttribute(ENTITY_ID));
                            workingEntity = entities[id];
                        }
                        else if(reader.Name == COMPONENT)
                        {
                            Type componentType = Type.GetType(reader.GetAttribute(COMPONENT_TYPE));
                            workingComponent = workingEntity.AddComponent(componentType) as ZodiacComponent;
                        }
                        else if(reader.Name == PROPERTY)
                        {
                            string propName = reader.GetAttribute(PROPERTY_NAME);
                            string propValString = reader.GetAttribute(PROPERTY_VALUE);
                            
                            PropertyInfo propInfo = workingComponent.GetType().GetProperty(propName);
                            Type propType = propInfo.PropertyType;

                            // if the type is a kind of list
                            if (typeof(IList).IsAssignableFrom(propType))
                            {
                                // do something i guess
                            }
                            else if(propType.IsEnum)
                            {
                                object propVal = Enum.Parse(propType, propValString);
                                propInfo.SetValue(workingComponent, propVal);
                            }
                            else if(propType == typeof(GameObject))
                            {
                                int id = int.Parse(propValString);
                                propInfo.SetValue(workingComponent, entities[id]);
                            }
                            else
                            {
                                object propVal = System.Convert.ChangeType(propValString, propType);
                                propInfo.SetValue(workingComponent, propVal);
                            }
                        }
                        break;
                    }
            }
        }

        return entities;
    }

    private void Clear()
    {
        ObjectIds.Clear();
        entityCount = 0;
    }

    private void WriteEntity(GameObject entity)
    {
        // assign a new id
        int id = GetId(entity);

        writer.WriteStartElement(ENTITY);
            // write id
            writer.WriteAttributeString(ENTITY_ID, id.ToString());
            // write all components
            foreach (ZodiacComponent comp in entity.GetComponents<ZodiacComponent>())
                comp.Serialize(this);
            
        writer.WriteEndElement();
    }
    public void WriteProperty(string propName, object value)
    {
        // dont write if null
        if (value == null)
            return;

        // write IDs instead of entities
        if (value is GameObject entity)
        {
            value = GetId(entity);
        }

        if(value is IEnumerable<GameObject> entityList)
        {
            int[] ids = new int[entityList.Count()];
            
            for(int i = 0; i < entityList.Count(); i++)
                ids[i] = GetId(entityList.ElementAt(i));

            value = ids;
        }

        // if the property is a collection, write all the elements in it
        if (value is IList collection)
        {
            Debug.Log($"{propName} is IList");

            // write a string of the items seperated by commas
            // value = collection.Cast<object>().Aggregate("", (result, next) => $"{result},{next}", final => final.TrimStart(','));
            
            // better idea, use recursion to write 'sub properties'
            writer.WriteStartElement(PROPERTY);
            writer.WriteAttributeString(PROPERTY_NAME, propName);
            foreach (object item in collection)
            {
                WriteProperty(propName, item);
            }
            writer.WriteEndElement();

            return;
        }
        
        // write
        writer.WriteStartElement(PROPERTY);
        writer.WriteAttributeString(PROPERTY_NAME, propName);
        writer.WriteAttributeString(PROPERTY_VALUE, value.ToString());
        writer.WriteEndElement();
    }
    private int GetId(GameObject entity)
    {
        // if this entity has yet to be referenced, assign it a new id
        if (!ObjectIds.ContainsKey(entity))
            ObjectIds.Add(entity, entityCount++);
        
        return ObjectIds[entity];
    }
    public void WriteStartComponent(string compType)
    {
        writer.WriteStartElement(COMPONENT);
        writer.WriteAttributeString(COMPONENT_TYPE, compType);
    }
    public void WriteEndComponent()
    {
        writer.WriteEndElement();
    }
}