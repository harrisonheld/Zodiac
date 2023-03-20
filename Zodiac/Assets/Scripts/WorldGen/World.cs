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

        // hash that will persist across application restarts
        // not very clever but works well enough - much like myself
        private static int StableHash(params int[] toHash)
        {
            /*
             * I am not good at this kind of math and cannot gaurantee this hash is any good
             * No I have not tested it
             */
            
            // djb2 algorithm
            int hash = 5381;
            for(int i = 0; i < toHash.Length; i++)
            {
                hash = unchecked(hash * 33 + toHash[i]);
            }

            return hash;
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

            CellularAutomata ca = new(SCREEN_WIDTH, SCREEN_HEIGHT);
            ca.Generate(random, gaps);

            List<GameObject> walls = new();
            foreach (Vector2Int wallPos in ca.WallCoordinates())
            {
                GameObject wall = new GameObject();
                
                wall.AddComponent<Position>();
                wall.GetComponent<Position>().Pos = wallPos;

                wall.AddComponent<Visual>();
                var vis = wall.GetComponent<Visual>();
                bool flip = random.NextDouble() <= 0.7;
                vis.Sprite = flip ? "dots3x3" : "dots2x2";
                vis.ColorPrimary = (flip ? new Color(1f, 1f, .89f) : new Color(.99f, .99f, .92f));
                vis.ColorSecondary = (flip ? new Color(.76f, .76f, .63f) : new Color(.76f, .76f, .69f));
                vis.DisplayName = flip ? "oolitic limestone wall" : "sun-bleached oolitic limestone wall";
                vis.Description = "a sedimentary material. the oolites, which are small spherical grains of rock, appear to move and shift like a school of fish";
                if (!flip)
                    vis.Description += ". It has weathered many years of direct exposure to the sun";

                wall.AddComponent<Health>();

                wall.AddComponent<PhysicalAttributes>();

                walls.Add(wall);
            }

            return walls;
        }
        private enum Direction
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }
    }
}
