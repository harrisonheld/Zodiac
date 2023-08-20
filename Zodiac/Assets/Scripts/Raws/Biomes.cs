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
using WorldGen;

namespace Raws
{
    public static class Biomes
    {
        private const string BIOMES_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\Biomes\";

        private static Dictionary<string, BiomeInfo> _biomes = new();
        private static bool _initialized = false;
        public static void Initialize()
        {
            _biomes.Clear();

            foreach (string file in Directory.GetFiles(BIOMES_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fullPath = Path.Combine(BIOMES_DIR, file);
                string json = File.ReadAllText(fullPath);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                List<BiomeInfo> biomesInFile = JsonConvert.DeserializeObject<List<BiomeInfo>>(json, settings);

                foreach (BiomeInfo biome in biomesInFile)
                {
                    _biomes.Add(biome.Id, biome);
                }
            }

            _initialized = true;
        }

        public static BiomeInfo ById(string biomeId)
        {
            if (!_initialized)
                Initialize();

            return _biomes[biomeId];
        }
    }

    public class BiomeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null;
        [JsonConverter(typeof(ColorConverter))]
        public Color ColorBackground { get; set; }
        [JsonConverter(typeof(ColorConverter))]
        public Color ColorCursor { get; set; }
        [JsonConverter(typeof(ColorConverter))]
        public Color ColorPortrait { get; set; }
    }

    public class ColorConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string colorValue = reader.Value.ToString();

                if (colorValue.StartsWith("#") && colorValue.Length == 7) // #ABCDEF format
                {
                    Color color;
                    if(ColorUtility.TryParseHtmlString(colorValue, out color))
                        return color;
                    throw new JsonException($"Invalid Color: {colorValue}");
                }
            }

            throw new JsonException("Unexpected token or format while deserializing Color.");
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}