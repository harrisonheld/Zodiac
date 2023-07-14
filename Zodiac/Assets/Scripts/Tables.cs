using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Purchasing.MiniJSON;

public static class Tables
{
    private const string TABLES_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\Tables\";

    private static Dictionary<string, LootTable> _tables = new Dictionary<string, LootTable>();
    private static bool _initialized = false;
    private static void Initialize()
    {
        _initialized = true;

        // load all loot tables from file
        foreach(string file in Directory.GetFiles(TABLES_DIR))
        {
            if (!file.EndsWith(".json"))
                continue;

            string tableName = Path.GetFileNameWithoutExtension(file);
            string tablePath = Path.Combine(TABLES_DIR, file);

            string json = File.ReadAllText(tablePath);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new TableEntryConverter());
            List<LootTable> tablesInFile = JsonConvert.DeserializeObject<List<LootTable>>(json, settings);

            foreach(LootTable table in tablesInFile)
            {
                _tables.Add(table.TableName, table);
            }
        }
    }

    /// <summary>
    /// Get a blueprint from a given table.
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static string FromTable(string tableName)
    {
        if (!_initialized)
            Initialize();

        LootTable table = _tables[tableName];

        float totalWeight = table.Entries.Sum(entry => entry.Weight);
        float randomNumber = UnityEngine.Random.Range(0f, totalWeight);

        // Iterate through the loot entries and find the selected entry based on weight
        foreach (var entry in table.Entries)
        {
            randomNumber -= entry.Weight;

            if (randomNumber <= 0)
            {
                if (entry is BlueprintEntry itemEntry)
                {
                    return itemEntry.Blueprint;
                }
                else if (entry is TableReference referenceEntry)
                {
                    return FromTable(referenceEntry.TableName);
                }

                break;
            }
        }

        return "";
    }

    private abstract class EntryBase
    {
        [JsonProperty("weight")]
        public int Weight { get; set; }
    }
    private class BlueprintEntry : EntryBase
    {
        [JsonProperty("blueprint")]
        public string Blueprint { get; set; }
    }
    private class TableReference : EntryBase
    {
        [JsonProperty("table")]
        public string TableName { get; set; }
    }
    private class LootTable
    {
        [JsonProperty("tableName")]
        public string TableName { get; set; }
        [JsonProperty("entries")]
        public List<EntryBase> Entries { get; set; }
    }



    private class TableEntryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EntryBase);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            // Check if the entry is a blueprint or a reference to another table
            if (jsonObject.ContainsKey("blueprint"))
            {
                return jsonObject.ToObject<BlueprintEntry>();
            }
            else if (jsonObject.ContainsKey("table"))
            {
                return jsonObject.ToObject<TableReference>();
            }

            throw new JsonSerializationException("Invalid loot entry");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}