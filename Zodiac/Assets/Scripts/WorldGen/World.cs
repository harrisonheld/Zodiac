using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WorldGen
{
    public static class World
    {
        private static int worldSeed = 0;
        public static void SetWorldSeed(int newSeed)
        {
            worldSeed = newSeed;
        }

        // hash that will persist across application restarts
        // not very clever but works well enough - much like myself
        private static int StableHash(params int[] ints)
        {
            /*
             * I am not good at this kind of math and cannot gaurantee this hash is any good
             * No I have not tested it
             */
            
            // djb2 algorithm
            int hash = 5381;
            for(int i = 0; i < ints.Length; i++)
            {
                hash = unchecked(hash * 33 + ints[i]);
            }

            return hash;
        }
        public static List<GameObject> GenerateScreen(int x, int y)
        {
            CellularAutomata ca = new(Constants.GAMEAREA_WIDTH, Constants.GAMEAREA_HEIGHT);
            int screenSeed = ScreenSeed(x, y);
            System.Random generationRandom = new System.Random(screenSeed);
            ca.Generate(generationRandom);

            List<GameObject> walls = new();
            foreach (Vector2Int wallPos in ca.WallCoordinates())
            {
                GameObject wall = new GameObject();
                
                wall.AddComponent<Position>();
                wall.GetComponent<Position>().Pos = wallPos;

                wall.AddComponent<Visual>();
                var vis = wall.GetComponent<Visual>();
                bool flip = generationRandom.NextDouble() <= 0.7;
                vis.Sprite = flip ? "dots3x3" : "dots2x2";
                vis.SetColorPrimary(flip ? new Color(1f, 1f, .89f) : new Color(.99f, .99f, .92f));
                vis.SetColorSecondary(flip ? new Color(.76f, .76f, .63f) : new Color(.76f, .76f, .69f));
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
            
        private static int ScreenSeed(int x, int y)
        {
            return StableHash(worldSeed, x, y).GetHashCode();
        }
    }
}
