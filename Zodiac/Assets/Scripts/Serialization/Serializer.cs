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

public class Serializer
{
    public void SerializeScene(string path)
    {
        using FileStream stream = new FileStream(path, FileMode.Create);
        using BinaryWriter writer = new BinaryWriter(stream);

        var entities = GameObject.FindObjectsOfType<ZodiacComponent>()
                        .Select(c => c.gameObject)
                        .Distinct();

        writer.Write(entities.Count());

        foreach (var entity in entities)
        {
            writer.Write(entity.GetInstanceID());
            writer.Write(entity.GetComponents<ZodiacComponent>().Count());

            foreach (var component in entity.GetComponents<ZodiacComponent>())
            {
                writer.Write(component.GetType().FullName);
                component.Serialize(writer);
            }
        }
    }
}