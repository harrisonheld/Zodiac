using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGen
{
	internal class CellularAutomata
	{
		private readonly int[][] DIRECTIONS = new int[][] {
			new int[] {1, 0},
			new int[] {-1, 0},
			new int[] {0, 1},
			new int[] {0, -1}
		};

		private int iterations;
		private double fillPercent;
		private int width, height;
		private Cell[,] cells;

		private List<Cell> cavernCells = new List<Cell>();
		private int caverns = int.MaxValue;

		private System.Random layoutRand = new();

		public CellularAutomata(int _width, int _height, int _iterations = 4, double _fillPercent = 0.50)
		{
			width = _width;
			height = _height;
			iterations = _iterations;
			fillPercent = _fillPercent;

			cells = new Cell[width, height];
		}
		public void Generate()
		{
			//create random cells
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Cell newCell = new Cell();
					newCell.x = x;
					newCell.y = y;

					if (x != 0 && y != 0 && x != width - 1 && y != height - 1)
						newCell.type = layoutRand.NextDouble() > fillPercent ? CellType.Floor : CellType.Wall;
					else
						newCell.type = CellType.Wall;

					cells[x, y] = newCell;
				}
			}

			//smoothing
			for (int i = 0; i < iterations; i++)
			{
				for (int y = 1; y < height - 1; y++)
				{
					for (int x = 1; x < width - 1; x++)
					{
						cells[x, y].neighbors = 0;

						for (int x1 = x - 1; x1 < x + 2; x1++)
						{
							for (int y1 = y - 1; y1 < y + 2; y1++)
							{
								if (!(x1 == x && y1 == y) && cells[x1, y1].type == CellType.Floor)
								{
									cells[x, y].neighbors++;
								}
							}
						}
					}
				}

				for (int y = 1; y < height - 1; y++)
				{
					for (int x = 1; x < width - 1; x++)
					{
						if (cells[x, y].neighbors > 4)
							cells[x, y].type = CellType.Floor;
						else if (cells[x, y].neighbors < 4)
							cells[x, y].type = CellType.Wall;
					}
				}
			}

			//connect caverns
			while (caverns > 1)
			{
				FindCaverns();

				Cell cell1 = cavernCells[0];
				Cell cell2 = cavernCells[cavernCells.Count - 1];

				List<Cell> path = FindPath(cell1.x, cell1.y, cell2.x, cell2.y);

				foreach (Cell p in path)
				{
					if (p.type == CellType.Wall)
					{
						p.type = CellType.Path;
					}
				}
			}
		}
		public void Generate(System.Random rand)
		{
            layoutRand = rand;
			Generate();
		}

		public IEnumerable<Vector2Int> WallCoordinates()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (cells[x, y].type == CellType.Wall)
						yield return new Vector2Int(x, y);
				}
			}
		}

		private void FindCaverns()
		{
			cavernCells = new List<Cell>();
			caverns = 0;

			for (int y = 1; y < height - 1; y++)
			{
				for (int x = 1; x < width - 1; x++)
				{
					cells[x, y].marked = false;
				}
			}

			for (int y = 1; y < height - 1; y++)
			{
				for (int x = 1; x < width - 1; x++)
				{
					if ((cells[x, y].type == CellType.Floor || cells[x, y].type == CellType.Path) && !cells[x, y].marked)
					{
						caverns++;
						cavernCells.Add(cells[x, y]);

						Queue<Cell> queue = new Queue<Cell>();
						queue.Enqueue(cells[x, y]);

						while (queue.Count > 0)
						{
							Cell cell = queue.Dequeue();
							cell.cavernIndex = caverns - 1;

							for (int i = 0; i < DIRECTIONS.Length; i++)
							{
								int[] locCoord = DIRECTIONS[i];

								Cell neighbor = cells[cell.x + locCoord[0], cell.y + locCoord[1]];

								if ((neighbor.type == CellType.Floor || neighbor.type == CellType.Path) && !neighbor.marked)
								{
									neighbor.marked = true;
									queue.Enqueue(neighbor);
								}
							}
						}
					}
				}
			}
		}
		private List<Cell> FindPath(int x1, int y1, int x2, int y2)
		{
			List<Cell> closedSet = new List<Cell>();
			List<Cell> openSet = new List<Cell>() { cells[x1, y1] };

			Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();

			int[,] gScore = new int[width, height]; //cost from start to this node
			int[,] fScore = new int[width, height]; //cost from this node to goal

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					gScore[x, y] = int.MaxValue;
					fScore[x, y] = int.MaxValue;
				}
			}

			gScore[x1, y1] = 0;
			fScore[x1, y1] = CostHueristic(x1, y1, x2, y2);

			while (openSet.Count > 0)
			{
				int lowestFScore = int.MaxValue;
				Cell current = null;

				foreach (Cell node in openSet)
				{
					if (fScore[node.x, node.y] < lowestFScore)
					{
						lowestFScore = fScore[node.x, node.y];
						current = node;
					}
				}

				if (current.x == x2 && current.y == y2)
				{
					return ReconstructPath(cameFrom, current);
				}

				openSet.Remove(current);
				closedSet.Add(current);

				for (int i = 0; i < DIRECTIONS.Length; i++)
				{
					int[] locCoord = DIRECTIONS[i];

					Cell neighbor = cells[current.x + locCoord[0], current.y + locCoord[1]];

					if (neighbor.x < 1 || neighbor.y < 1 || neighbor.x >= width - 1 || neighbor.y >= height - 1) //this cell is on an edge
						continue;

					if (closedSet.Contains(neighbor))
						continue;

					if (!openSet.Contains(neighbor))
						openSet.Add(neighbor);

					int tentativeGScore = gScore[neighbor.x, neighbor.y];

					switch (neighbor.type)
					{
						case CellType.Floor:
							tentativeGScore += 1;
							break;
						case CellType.Wall:
							tentativeGScore += 2;
							break;
						case CellType.Path:
							tentativeGScore += 1;
							break;
						default:
							tentativeGScore += 1;
							break;
					}

					if (tentativeGScore >= gScore[neighbor.x, neighbor.y])
						continue;

					cameFrom[neighbor] = current;
					gScore[neighbor.x, neighbor.y] = tentativeGScore;
					fScore[neighbor.x, neighbor.y] = gScore[neighbor.x, neighbor.y] + CostHueristic(neighbor.x, neighbor.y, x2, y2);
				}
			}
			//if this point is reached, could not find path
			return null;
		}

		private int CostHueristic(int x1, int y1, int x2, int y2)
		{
			int cost = Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
			return cost;
		}

		private int DistBetween(int x1, int y1, int x2, int y2)
		{
			// taxicab distance
			int dist = Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
			return dist;
		}

		private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
		{
			List<Cell> totalPath = new List<Cell>() { current };

			while (cameFrom.ContainsKey(current))
			{
				current = cameFrom[current];
				totalPath.Add(current);
			}

			return totalPath;
		}
		class Cell
		{
			public int x, y;
			public int neighbors;
			public int cavernIndex = -1;
			public bool marked = false;

			public CellType type;
		}
		enum CellType
		{
			Floor,
			Wall,
			Path
		}
	}
}