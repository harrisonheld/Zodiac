using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Raws;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Unity.Mathematics;

namespace WorldGen
{
    public static class World
    {
        private const int GAP_NONCE = 0;

        private static int _worldSeed = 0;

        public static void SetWorldSeed(int newSeed)
        {
            _worldSeed = newSeed;
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

            int zoneSeed = StableHash(x, y, _worldSeed);
            System.Random zoneRandom = new(zoneSeed);
            IZoneGenerator generator = SelectZoneGenerator(x, y);
            zoneInfo = generator.Generate(zoneRandom, gaps);

            zoneInfo.X = x;
            zoneInfo.Y = y;
            return zoneInfo;
        }

        private static IZoneGenerator SelectZoneGenerator(int x, int y)
        {
            return new DesertCanyonGenerator();
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
            // check if there is a handmade zone at the specified coordinates
            var thisZone = HandmadeZones.GetZone(x, y);
            if(thisZone != null)
            {
                if(dir == Direction.North)
                {
                    return thisZone.Gaps.north;
                }
                else if(dir == Direction.East)
                {
                    return thisZone.Gaps.east;
                }
                else if(dir == Direction.South)
                {
                    return thisZone.Gaps.south;
                }
                else if(dir == Direction.West)
                {
                    return thisZone.Gaps.west;
                }
            }

            // check if there is a handmade zone in the direction specified
            if (dir == Direction.North)
            {
                var zone = HandmadeZones.GetZone(x, y + 1);
                if (zone != null)
                    return zone.Gaps.south;
            }
            else if (dir == Direction.East)
            {
                var zone = HandmadeZones.GetZone(x + 1, y);
                if (zone != null)
                    return zone.Gaps.west;
            }
            else if (dir == Direction.South)
            {
                var zone = HandmadeZones.GetZone(x, y-1);
                if (zone != null)
                    return zone.Gaps.north;

                y--;
                dir = Direction.North;
            }
            else if (dir == Direction.West)
            {
                var zone = HandmadeZones.GetZone(x-1, y);
                if (zone != null)
                    return zone.Gaps.east;

                x--;
                dir = Direction.East;
            }

            int edgeSeed = StableHash(x, y, dir, GAP_NONCE);
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
