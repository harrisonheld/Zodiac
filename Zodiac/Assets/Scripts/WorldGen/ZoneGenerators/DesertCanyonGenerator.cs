using Raws;
using System;
using UnityEngine;
using UnityEngine.XR;
using WorldGen;

namespace WorldGen
{
	public class HallsOfGrayGenerator : IZoneGenerator
	{
		private string WALL_TABLE = "HallsOfGrayWalls";
		private string ENEMY_TABLE = "HallsOfGrayEnemies";
		private string BIOME_ID = "HallsOfGray";

		public ZoneInfo Generate(System.Random rand, Gaps gaps)
		{
			ZoneInfo info = new ZoneInfo();
			info.Width = 40;
			info.Height = 30;
			info.BiomeId = BIOME_ID;

			ITerrainGenerator terrainGenerator = new BSP();
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