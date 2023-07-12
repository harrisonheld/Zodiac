using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WorldGen
{
    public static class World
    {
        private const int WORLD_WIDTH = 30;
        private const int WORLD_HEIGHT = 30;

        public const int SCREEN_WIDTH = 28;
        public const int SCREEN_HEIGHT = 21;
        
        private static int worldSeed = 0;
        public static void SetWorldSeed(int newSeed)
        {
            worldSeed = newSeed;
        }

        public static List<GameObject> GenerateScreen(int x, int y)
        {
            Gaps gaps = new()
            {
                north = GenerateEdge(x, y, Direction.North),
                east = GenerateEdge(x, y, Direction.East),
                south = GenerateEdge(x, y, Direction.South),
                west = GenerateEdge(x, y, Direction.West)
            };

            int screenSeed = StableHash(worldSeed, x, y);
            System.Random random = new(screenSeed);

            ITerrainGenerator generator = GetTerrainGenerator(x, y);
            generator.Generate(random, gaps);

            List<GameObject> entities = new();
            foreach (Vector2Int wallPos in generator.WallCoordinates())
            {
                string blueprint = random.NextDouble() <= 0.7 ? "LimestoneWall" : "LimestoneWallAlt";
                GameObject wall = EntitySerializer.EntityFromBlueprint(blueprint, wallPos);
                entities.Add(wall);
            }
            foreach (Vector2Int pathPos in generator.PathCoordinates())
            {
                GameObject path = EntitySerializer.EntityFromBlueprint("Path", pathPos);
                entities.Add(path);
            }

            return entities;
        }
        // pick a terrain generator based on the screen's position
        private static ITerrainGenerator GetTerrainGenerator(int x, int y)
        {
            return new CellularAutomata(SCREEN_WIDTH, SCREEN_HEIGHT);
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
                    hash = hash * 33 + toHash[i];
                }

                return hash;
            }
        }
        private static InclusiveIntRange GenerateEdge(int x, int y, Direction dir)
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
            int GAP_MIN = 3;
            int GAP_MAX = 7;
            int bound = (dir == Direction.North) ? SCREEN_WIDTH : SCREEN_HEIGHT;

            int gapWidth = rand.Next(GAP_MIN, GAP_MAX + 1);
            int gap1 = rand.Next(0, bound - gapWidth);
            int gap2 = gap1 + gapWidth;
            return new InclusiveIntRange(gap1, gap2);
        }
    }
}
