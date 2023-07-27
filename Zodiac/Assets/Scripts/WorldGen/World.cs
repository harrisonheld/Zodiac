using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Raws;

namespace WorldGen
{
    public static class World
    {
        private static ZoneInfo _currentZone;
        public static int GetCurrentZoneWidth => _currentZone.Width;
        public static int GetCurrentZoneHeight => _currentZone.Height;

        private static int worldSeed = 0;
        public static void SetWorldSeed(int newSeed)
        {
            worldSeed = newSeed;
        }

        public static List<GameObject> GenerateZone(int x, int y)
        {
            GenerateZoneInfo(x, y);
            _currentZone.Generator.Generate(_currentZone.ZoneRandom, _currentZone.Gaps);
            // write all the gap spaces to log
            Debug.Log(
                $"Gaps: north: {_currentZone.Gaps.north.Start} - {_currentZone.Gaps.north.End},"+
                $" east: {_currentZone.Gaps.east.Start} - {_currentZone.Gaps.east.End},"+
                $" south: {_currentZone.Gaps.south.Start} - {_currentZone.Gaps.south.End},"+
                $" west: {_currentZone.Gaps.west.Start} - {_currentZone.Gaps.west.End}"
            );

            List<GameObject> entities = new();
            foreach (Vector2Int wallPos in _currentZone.Generator.WallCoordinates())
            {
                string blueprint = _currentZone.ZoneRandom.NextDouble() <= 0.7 ? "LimestoneWall" : "LimestoneWallAlt";
                GameObject wall = Blueprints.FromBlueprint(blueprint, wallPos);
                entities.Add(wall);
            }
            foreach (Vector2Int pathPos in _currentZone.Generator.PathCoordinates())
            {
                GameObject path = Blueprints.FromBlueprint("Path", pathPos);
                entities.Add(path);
            }

            return entities;
        }
        private static void GenerateZoneInfo(int x, int y)
        {
            Gaps gaps = new()
            {
                north = GenerateEdge(x, y, Direction.North),
                east = GenerateEdge(x, y, Direction.East),
                south = GenerateEdge(x, y, Direction.South),
                west = GenerateEdge(x, y, Direction.West)
            };

            int zoneSeed = StableHash(x, y, worldSeed);

            _currentZone = new ZoneInfo();
            _currentZone.ZoneRandom = new System.Random(zoneSeed);
            _currentZone.Gaps = gaps;
            _currentZone.Width = (_currentZone.ZoneRandom.Next(6) + 4) * 4;
            _currentZone.Height = _currentZone.Width / 4 * 3;
            _currentZone.Generator = new CellularAutomata(_currentZone.Width, _currentZone.Height);
        }

        // hash that will persist across application restarts
        // untested but appears to work well enough - much like myself
        private static int StableHash(params int[] toHash)
        {
            // djb2 algorithm
            unchecked
            {
                int hash = 5381;
                for (int i = 0; i < toHash.Length; i++)
                {
                    // fun fact: nobody knows why the number 33 works so well. it just does
                    hash = hash * 33 + toHash[i];
                }

                return hash;
            }
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

            int seed = StableHash(x, y, (int)dir);

            // is constructing new Randoms this frequently a good idea?
            // a problem for a future harrison.
            System.Random rand = new(seed);
            double GAP_MIN = 0.3;
            double GAP_MAX = 0.7;

            double gapWidth = rand.NextDouble() * (GAP_MAX - GAP_MIN) + GAP_MIN;
            double gapStart = rand.NextDouble() * (1.0 - gapWidth);
            double gapEnd = gapStart + gapWidth;
            return new InclusiveDoubleRange(gapStart, gapEnd);
        }
    }
}
