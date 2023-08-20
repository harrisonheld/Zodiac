using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Raws;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace WorldGen
{
    public static class World
    {
        private static int worldSeed = 0;
        public static void SetWorldSeed(int newSeed)
        {
            worldSeed = newSeed;
        }

        public static ZoneInfo GenerateZone(int x, int y)
        {
            Gaps gaps = new()
            {
                north = GenerateEdge(x, y, Direction.North),
                east = GenerateEdge(x, y, Direction.East),
                south = GenerateEdge(x, y, Direction.South),
                west = GenerateEdge(x, y, Direction.West)
            };

            ZoneInfo zoneInfo = HandmadeZones.TryInstantiateZone(x, y);
            if (zoneInfo != null)
            {
                return zoneInfo;
            }

            int zoneSeed = StableHash(x, y, worldSeed);
            System.Random zoneRandom = new(zoneSeed);
            ITerrainGenerator generator = SelectTerrainGenerator(x, y);
            generator.Generate(zoneRandom, gaps);
            zoneInfo = generator.GetZoneInfo();

            foreach (Vector2Int wallPos in generator.WallCoordinates())
            {
                string blueprint = zoneRandom.NextDouble() <= 0.7 ? "LimestoneWall" : "LimestoneWallAlt";
                GameObject wall = Blueprints.FromBlueprint(blueprint, wallPos);
            }
            foreach (Vector2Int pathPos in generator.PathCoordinates())
            {
                GameObject path = Blueprints.FromBlueprint("Path", pathPos);
            }
            foreach(Vector2Int enemyPos in generator.EnemyCoordinates())
            {
                double r = zoneRandom.NextDouble();
                GameObject enemy = null;
                if (r > 0.66)
                    enemy = Blueprints.FromBlueprint("EnthralledAlchemist", enemyPos);
                else if (r > 0.33)
                    enemy = Blueprints.FromBlueprint("PorcelainFrog", enemyPos);
                else
                    enemy = Blueprints.FromBlueprint("LanguishingSmoke", enemyPos);
            }

            zoneInfo.X = x;
            zoneInfo.Y = y;
            zoneInfo.BiomeId = "DesertCanyon";
            return zoneInfo;
        }
        private static ITerrainGenerator SelectTerrainGenerator(int x, int y)
        {
            return new CellularAutomata();
        }

        // hash that will persist across application restarts
        // untested but appears to work well enough - much like myself
        private static int DJB2Hash(params int[] toHash)
        {
            // djb2 algorithm
            unchecked
            {
                int hash = 5381;
                for (int i = 0; i < toHash.Length; i++)
                {
                    hash = hash * 33 + toHash[i];
                }

                return hash;
            }
        }
        private static int StableHash(params object[] toHash)
        {
            // get the raw bytes of the objects
            using MemoryStream stream = new();
            BinaryFormatter formatter = new();
            foreach(object obj in toHash)
            {
                formatter.Serialize(stream, obj);
            }
            byte[] dataBytes = stream.ToArray();

            // sha256 babey
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(dataBytes);
            // Convert the first 4 bytes of the hash to an integer
            int hashValue = BitConverter.ToInt32(hashBytes, 0);
            return hashValue;
        }

        private static InclusiveDoubleRange GenerateEdge(int x, int y, Direction dir)
        {
            if (dir == Direction.South)
            {
                y--;
                dir = Direction.North;
            }
            else if (dir == Direction.West)
            {
                x--;
                dir = Direction.East;
            }

            int edgeSeed = StableHash(x, y, dir);

            // is constructing new Randoms this frequently a good idea?
            // a problem for a future harrison.
            System.Random rand = new(edgeSeed);
            double GAP_MIN = 0.3;
            double GAP_MAX = 0.7;

            double gapWidth = rand.NextDouble() * (GAP_MAX - GAP_MIN) + GAP_MIN;
            double gapStart = rand.NextDouble() * (1.0 - gapWidth);
            double gapEnd = gapStart + gapWidth;
            return new InclusiveDoubleRange(gapStart, gapEnd);
        }
    }
}
