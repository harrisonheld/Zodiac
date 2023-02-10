using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

public class EntitySerializer2
{
    private const string ENTITIES = "Entities"; // the start marker for all entities
    private const string ENTITY = "Entity"; // the start marker for an entity
    private const string ENTITY_ID = "ID"; // used to identify objects. entity references will use this
    private const string ENTITY_COUNT = "EntityCount"; // used to say how many entities are in the doc

    private readonly XmlWriterSettings writerSettings = new()
    {
        Indent = true,
        IndentChars = "\t",
        NewLineOnAttributes = true,
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
    public ICollection<GameObject> DeserializeScene(string path)
    {
        Clear();
        
        reader = XmlReader.Create(inputUri:path, settings:readerSettings);

        // so we first have to create all the entities and assign them IDs so they will 
        // exist when it comes time to write the references in
        GameObject[] entities = null;

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
                            GameObject entity = entities[id];
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
        if (!ObjectIds.ContainsKey(entity))
            ObjectIds.Add(entity, entityCount++);
        int id = ObjectIds[entity];

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
            // if this entity has yet to be referenced, assign it a new id
            if (!ObjectIds.ContainsKey(entity))
                ObjectIds.Add(entity, entityCount++);
            // set value to the ID so we write that instead of the entity itself
            value = ObjectIds[entity];
        }

        // if the property is a collection, write all the elements in it
        if (value is IEnumerable<object> collection)
        {
            writer.WriteStartElement(propName);
            foreach (var item in collection)
            {
                WriteProperty(propName, item);
            }
            writer.WriteEndElement();
            return;
        }

        writer.WriteElementString(propName, value.ToString());
    }

    // wrappers
    public void WriteStartElement(string localName)
    {
        writer.WriteStartElement(localName);
    }
    public void WriteEndElement()
    {
        writer.WriteEndElement();
    }
}