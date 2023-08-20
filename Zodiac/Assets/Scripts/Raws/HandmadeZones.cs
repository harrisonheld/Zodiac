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
    public static class HandmadeZones
    {
        private const string ZONES_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\HandmadeZones\";

        private static Dictionary<(int, int), HandmadeZone> _zones = new();
        private static bool _initialized = false;
        public static void Initialize()
        {
            _zones.Clear();

            foreach (string file in Directory.GetFiles(ZONES_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fullPath = Path.Combine(ZONES_DIR, file);

                string json = File.ReadAllText(fullPath);
                HandmadeZone zone = JsonConvert.DeserializeObject<HandmadeZone>(json);
                _zones.Add((zone.WorldX, zone.WorldY), zone);
            }

            _initialized = true;
        }

        public static ZoneInfo TryInstantiateZone(int worldX, int worldY)
        {
            if (!_initialized)
                Initialize();

            if (!_zones.ContainsKey((worldX, worldY)))
                return null; // no zone at those coordinates

            HandmadeZone zone = _zones[(worldX, worldY)];
            foreach (HandplacedEntity e in zone.HandplaceEntities)
            {
                Blueprints.FromBlueprint(e.Blueprint, new Vector2Int(e.X, e.Y));
            }

            return new ZoneInfo()
            {
                Width = zone.Width,
                Height = zone.Height,
                X = worldX,
                Y = worldY,
                BiomeId = zone.BiomeId
            };
        }

        private class HandmadeZone
        {
            [JsonProperty("worldX")]
            public int WorldX { get; set; }
            [JsonProperty("worldY")]
            public int WorldY { get; set; }
            [JsonProperty("width")]
            public int Width { get; set; }
            [JsonProperty("height")]
            public int Height { get; set; }
            [JsonProperty("zoneName")]
            public string ZoneName { get; set; }
            [JsonProperty("biomeId")]
            public string BiomeId { get; set; }
            [JsonProperty("entities")]
            public List<HandplacedEntity> HandplaceEntities { get; set; }
        }

        private class HandplacedEntity
        {
            [JsonProperty("blueprint")]
            public string Blueprint { get; set; }
            [JsonProperty("x")]
            public int X { get; set; }
            [JsonProperty("y")]
            public int Y { get; set; }
        }
    }
}