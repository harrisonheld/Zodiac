using Raws;
using System;
using UnityEngine;
using UnityEngine.XR;
using WorldGen;

namespace WorldGen
{
	public class DesertCanyonGenerator : IZoneGenerator
	{
		private string WALL_TABLE = "DesertCanyonWalls";
		private string ENEMY_TABLE = "DesertCanyonEnemies";
		private string BIOME_ID = "DesertCanyon";

		public ZoneInfo Generate(System.Random rand, Gaps gaps)
		{
			ZoneInfo info = new ZoneInfo();
			info.Width = (rand.Next(6) + 4) * 4;
			info.Height = info.Width / 4 * 3;
			info.BiomeId = BIOME_ID;

			ITerrainGenerator terrainGenerator = new CellularAutomata();
			terrainGenerator.Generate(rand, info.Width, info.Height, gaps);

			foreach (Vector2Int wallPos in terrainGenerator.WallCoordinates())
			{
				string wall = Tables.FromTable(WALL_TABLE);
				Blueprints.FromBlueprint(wall, wallPos);
			}
			foreach (Vector2Int enemyPos in terrainGenerator.EnemyCoordinates())
			{
				string enemy = Tables.FromTable(ENEMY_TABLE);
				Blueprints.FromBlueprint(enemy, enemyPos);
			}

			return info;
		}
	}
}