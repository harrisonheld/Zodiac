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

public class ZoneContentDeserializer
{
    public List<GameObject> DeserializeScene(BinaryReader reader)
    {
        Dictionary<int, GameObject> idToEntity = new Dictionary<int, GameObject>();

        int entitiesCount = reader.ReadInt32();

        for(int i = 0; i < entitiesCount; i++)
        {
            int entityId = reader.ReadInt32();
            int componentsCount = reader.ReadInt32();

            if(!idToEntity.ContainsKey(entityId))
                idToEntity.Add(entityId, new GameObject());
            GameObject entity = idToEntity[entityId];

            for(int j = 0; j < componentsCount; j++)
            {
                string componentTypeFullName = reader.ReadString();
                Type componentType = Type.GetType(componentTypeFullName);
                ZodiacComponent component = entity.AddComponent(componentType) as ZodiacComponent;
                component.Deserialize(reader, idToEntity);
            }
        }

        return idToEntity.Values.ToList();
    }
}