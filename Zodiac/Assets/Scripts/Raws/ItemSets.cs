using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Raws
{
    public static class ItemSets
    {
        private const string BLUEPRINTS_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\ItemSets\";

        private static Dictionary<string, ItemSet> _itemSets = new();
        private static bool _initialized = false;
        public static void Initialize()
        {
            _itemSets.Clear();

            foreach (string file in Directory.GetFiles(BLUEPRINTS_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fullPath = Path.Combine(BLUEPRINTS_DIR, file);

                string json = File.ReadAllText(fullPath);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new ItemConverter());
                List<ItemSet> setsInFile = JsonConvert.DeserializeObject<List<ItemSet>>(json, settings);

                foreach (ItemSet set in setsInFile)
                {
                    _itemSets.Add(set.Name, set);
                }
            }

            _initialized = true;
        }

        public static List<GameObject> SpawnSet(string setName)
        {
            if(!_initialized)
                Initialize();

            ItemSet set = _itemSets[setName];
            List<GameObject> items = new List<GameObject>();

            foreach(ItemBase itemEntry in set.Items)
            {
                if(itemEntry is ItemFromBlueprint itemFromBlueprint)
                {
                    for(int i = 0; i < itemFromBlueprint.Count; i++)
                    {
                        items.Add(Blueprints.FromBlueprint(itemFromBlueprint.Blueprint));
                    }
                }
                else if(itemEntry is ItemFromTable itemFromTable)
                {
                    for (int i = 0; i < itemFromTable.Count; i++)
                    {
                        items.Add(Blueprints.FromBlueprint(Tables.FromTable(itemFromTable.TableName)));
                    }
                }
            }

            return items;
        }

        private class ItemSet
        {
            [JsonProperty("itemSetName")]
            public string Name { get; set; }
            [JsonProperty("items")]
            public List<ItemBase> Items { get; set; }
        }
        private abstract class ItemBase
        {
            public int Count { get; set; } = 1;
        }
        private class ItemFromBlueprint : ItemBase
        {
            [JsonProperty("blueprint")]
            public string Blueprint { get; set; }
        }
        private class ItemFromTable : ItemBase
        {
            [JsonProperty("table")]
            public string TableName { get; set; }
        }



        private class ItemConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ItemBase);
            }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);

                // Check if the entry is a blueprint or a reference to another table
                if (jsonObject.ContainsKey("blueprint"))
                {
                    return jsonObject.ToObject<ItemFromBlueprint>();
                }
                else if (jsonObject.ContainsKey("table"))
                {
                    return jsonObject.ToObject<ItemFromTable>();
                }

                throw new JsonSerializationException("Invalid json item.");
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}