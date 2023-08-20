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
                settings.Converters.Add(new ColorConverter());
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

        private class ColorConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Color);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string colorString = (string)reader.Value;
                    Color color;
                    if (ColorUtility.TryParseHtmlString(colorString, out color))
                        return color;
                }

                throw new JsonSerializationException("Invalid color value.");
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class BiomeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null;
        public Color BackgroundColor { get; set; }
        public Color CursorColor { get; set; }
        public Color PortraitColor { get; set; }
    }
}