using System.Collections.Generic;
using UnityEngine;
using WorldGen;

namespace WorldGen
{
    internal interface ITerrainGenerator
    {
        public void Generate(System.Random random, Gaps gaps);
        public ZoneInfo GetZoneInfo();

        public IEnumerable<Vector2Int> WallCoordinates();
        public IEnumerable<Vector2Int> FloorCoordinates();
        public IEnumerable<Vector2Int> PathCoordinates();
        public IEnumerable<Vector2Int> EnemyCoordinates();
    }
}