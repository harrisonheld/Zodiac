using System;
using System.Collections.Generic;
using UnityEngine;
using WorldGen;

namespace WorldGen
{
	internal class CellularAutomata : ITerrainGenerator
	{
		private readonly int[][] DIRECTIONS = new int[][] {
			new int[] {1, 0},
			new int[] {-1, 0},
			new int[] {0, 1},
			new int[] {0, -1}
		};

		private int _iterations;
		private double _fillPercent;
		private int _width, _height;
		private Cell[,] _cells;

		private List<Cell> _cavernCells = new List<Cell>();
		private int _cavernCount = int.MaxValue;

		private System.Random _rand = null;

		public CellularAutomata(int _width, int _height, int _iterations = 4, double _fillPercent = 0.50)
		{
			this._width = _width;
			this._height = _height;
			this._iterations = _iterations;
			this._fillPercent = _fillPercent;

            _cells = new Cell[this._width, this._height];
		}
		private void Generate(Gaps gaps)
		{
			//create random cells
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					Cell newCell = new Cell();
					newCell.x = x;
					newCell.y = y;

                    // gaps
                    float u = (float)x / _width;
					float v = (float)y / _height;
                    if (y == _height - 1 && gaps.north.Contains(u) ||
							x == _width - 1 && gaps.east.Contains(v) ||
							y == 0 && gaps.south.Contains(u) ||
							x == 0 && gaps.west.Contains(v))
                        newCell.type = CellType.Floor;
                    // walls on edges
                    else if (x == 0 || y == 0 || x == _width - 1 || y == _height - 1)
						newCell.type = CellType.Wall;
					else
						newCell.type = _rand.NextDouble() > _fillPercent ? CellType.Floor : CellType.Wall;

					_cells[x, y] = newCell;
				}
			}

			//smoothing
			for (int i = 0; i < _iterations; i++)
			{
				for (int y = 1; y < _height - 1; y++)
				{
					for (int x = 1; x < _width - 1; x++)
					{
						_cells[x, y].neighbors = 0;

						for (int x1 = x - 1; x1 < x + 2; x1++)
						{
							for (int y1 = y - 1; y1 < y + 2; y1++)
							{
								if (!(x1 == x && y1 == y) && _cells[x1, y1].type == CellType.Floor)
								{
									_cells[x, y].neighbors++;
								}
							}
						}
					}
				}

				for (int y = 1; y < _height - 1; y++)
				{
					for (int x = 1; x < _width - 1; x++)
					{
						if (_cells[x, y].neighbors > 4)
							_cells[x, y].type = CellType.Floor;
						else if (_cells[x, y].neighbors < 4)
							_cells[x, y].type = CellType.Wall;
					}
				}
			}

			//connect caverns
			while (_cavernCount > 1)
			{
				FindCaverns();

				Cell cell1 = _cavernCells[0];
				Cell cell2 = _cavernCells[_cavernCells.Count - 1];

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
		public void Generate(System.Random rand, Gaps gaps)
		{
            _rand = rand;
			Generate(gaps);
		}

		public IEnumerable<Vector2Int> WallCoordinates()
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					if (_cells[x, y].type == CellType.Wall)
						yield return new Vector2Int(x, y);
				}
			}
		}
		public IEnumerable<Vector2Int> FloorCoordinates()
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					if (_cells[x, y].type == CellType.Floor)
						yield return new Vector2Int(x, y);
				}
			}
		}
		public IEnumerable<Vector2Int> PathCoordinates()
		{
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					if (_cells[x, y].type == CellType.Path)
						yield return new Vector2Int(x, y);
				}
			}
		}
		public IEnumerable<Vector2Int> EnemyCoordinates()
		{
			var up = new NotImplementedException("i don't feel so good...");
			throw up;
		}
        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        private void FindCaverns()
		{
			_cavernCells = new List<Cell>();
			_cavernCount = 0;

			for (int y = 1; y < _height - 1; y++)
			{
				for (int x = 1; x < _width - 1; x++)
				{
					_cells[x, y].marked = false;
				}
			}

			for (int y = 1; y < _height - 1; y++)
			{
				for (int x = 1; x < _width - 1; x++)
				{
					if ((_cells[x, y].type == CellType.Floor || _cells[x, y].type == CellType.Path) && !_cells[x, y].marked)
					{
						_cavernCount++;
						_cavernCells.Add(_cells[x, y]);

						Queue<Cell> queue = new Queue<Cell>();
						queue.Enqueue(_cells[x, y]);

						while (queue.Count > 0)
						{
							Cell cell = queue.Dequeue();
							cell.cavernIndex = _cavernCount - 1;

							for (int i = 0; i < DIRECTIONS.Length; i++)
							{
								int[] locCoord = DIRECTIONS[i];
								int neighborX = cell.x + locCoord[0];
								int neighborY = cell.y + locCoord[1];
								if (!InBounds(neighborX, neighborY))
									continue;

								Cell neighbor = _cells[neighborX, neighborY];

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
			List<Cell> openSet = new List<Cell>() { _cells[x1, y1] };

			Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();

			int[,] gScore = new int[_width, _height]; //cost from start to this node
			int[,] fScore = new int[_width, _height]; //cost from this node to goal

			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
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

					Cell neighbor = _cells[current.x + locCoord[0], current.y + locCoord[1]];

					if (neighbor.x < 1 || neighbor.y < 1 || neighbor.x >= _width - 1 || neighbor.y >= _height - 1) //this cell is on an edge
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
		
		private class Cell
		{
			public int x, y;
			public int neighbors;
			public int cavernIndex = -1;
			public bool marked = false;

			public CellType type;
		}
		private enum CellType
		{
			Floor,
			Wall,
			Path
		}
	}
}