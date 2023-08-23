using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR;
using WorldGen;

namespace WorldGen
{
	internal class BSP : ITerrainGenerator
	{
        private int _width;
        private int _height;
        private int _minLeafSize;
        private int _minRoomSize;
        private int _maxEnemiesPerRoom;
        private double _enemySpawnChance;
        private CellType[,] _cells;
        private System.Random _rand;

        public BSP(int minLeafSize = 6, int minRoomSize = 3, int maxEnemiesPerRoom = 3, double enemySpawnChance = 0.5f)
        {
            _minLeafSize = minLeafSize;
            _minRoomSize = minRoomSize;
            _maxEnemiesPerRoom = maxEnemiesPerRoom;
            _enemySpawnChance = enemySpawnChance;
        }

        public void Generate(System.Random rand, int width, int height, Gaps gaps)
        {
            _rand = rand;
            _width = width;
            _height = height;

            _cells = new CellType[_width, _height];

            Leaf root = new Leaf(0, 0, _width, _height, _minLeafSize);
            List<Leaf> leaves = new() { root };

            // splitting
            bool didSplit = true;
            while (didSplit)
            {
                for (int l = 0; l < leaves.Count; l++)
                {
                    didSplit = leaves[l].Split(_rand);

                    if (didSplit)
                    {
                        leaves.Add(leaves[l].Child1);
                        leaves.Add(leaves[l].Child2);
                    }
                }
            }

            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++)
                {
                    _cells[x, y] = CellType.Wall;
                }
            }
            CreateRooms(root);
        }

        private void CreateRooms(Leaf leaf)
        {
            if (leaf == null)
                return;

            if(leaf.Child1 != null || leaf.Child2 != null)
            {
                CreateRooms(leaf.Child1);
                CreateRooms(leaf.Child2);

                if(leaf.Child1 != null && leaf.Child2 != null)
                {
                    CreateHall(leaf.Child1, leaf.Child2);
                }

                return;
            }

            int roomWidth = _rand.Next(_minRoomSize, leaf.Width - 1);
            int roomHeight = _rand.Next(_minRoomSize, leaf.Height - 1);
            int roomX = _rand.Next(1, leaf.Width - roomWidth - 1);
            int roomY = _rand.Next(1, leaf.Height - roomHeight - 1);

            for (int y1 = 0; y1 < roomHeight; y1++)
            {
                for (int x1 = 0; x1 < roomWidth; x1++)
                {
                    _cells[leaf.X + roomX + x1, leaf.Y + roomY + y1] = CellType.Floor;
                }
            }

            bool doEnemySpawn = _rand.NextDouble() < _enemySpawnChance;
            if(doEnemySpawn)
            {
                int enemiesToSpawn = _rand.Next(1, _maxEnemiesPerRoom+1);
                int enemiesSpawned = 0;
                while(enemiesSpawned < enemiesToSpawn)
                {
                    int enemyX = _rand.Next(leaf.X + roomX, leaf.X + roomX + roomWidth);
                    int enemyY = _rand.Next(leaf.Y + roomY, leaf.Y + roomY + roomHeight);
                    if (_cells[enemyX, enemyY] != CellType.Enemy)
                    {
                        _cells[enemyX, enemyY] = CellType.Enemy;
                        enemiesSpawned++;
                    }
                }
            }
        }
        private void CreateHall(Leaf leaf1, Leaf leaf2)
        {
            Vector2Int center1 = new Vector2Int(leaf1.X + leaf1.Width / 2, leaf1.Y + leaf1.Height / 2);
            Vector2Int center2 = new Vector2Int(leaf2.X + leaf2.Width / 2, leaf2.Y + leaf2.Height / 2);
            CreatePath(center1, center2);
        }
        private void CreatePath(Vector2Int pos1, Vector2Int pos2)
        {
            Vector2Int direction = pos2 - pos1;
            Vector2Int step = new Vector2Int(Math.Sign(direction.x), Math.Sign(direction.y));

            Vector2Int currentPosition = pos1;
            while (currentPosition != pos2)
            {
                if (_cells[currentPosition.x, currentPosition.y] == CellType.Wall)
                    _cells[currentPosition.x, currentPosition.y] = CellType.Path;
                if (_cells[currentPosition.x + step.x, currentPosition.y] == CellType.Wall)
                    _cells[currentPosition.x + step.x, currentPosition.y] = CellType.Path;
                currentPosition += step;
            }
        }

        public IEnumerable<Vector2Int> WallCoordinates()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_cells[x, y] == CellType.Wall)
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
                    if (_cells[x, y] == CellType.Floor)
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
                    if (_cells[x, y] == CellType.Path)
                        yield return new Vector2Int(x, y);
                }
            }
        }
        public IEnumerable<Vector2Int> EnemyCoordinates()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_cells[x, y] == CellType.Enemy)
                        yield return new Vector2Int(x, y);
                }
            }
        }

        private class Leaf
        {
            public int Height;
            public int Width;
            public int X;
            public int Y;
            public int MinLeafSize;

            public Leaf Child1 = null;
            public Leaf Child2 = null;

            public Leaf(int x, int y, int width, int height, int minLeafSize)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                MinLeafSize = minLeafSize;
            }

            public bool Split(System.Random rand)
            {
                bool splitHorizontally;
                if (Width / Height >= 1.25)
                    splitHorizontally = false;
                else if (Height / Width >= 1.25)
                    splitHorizontally = true;
                else
                    splitHorizontally = rand.NextDouble() > 0.5;

                int max = (splitHorizontally ? Height : Width) - MinLeafSize;
                if (max <= MinLeafSize)
                    return false; //too small to split

                int split = rand.Next(MinLeafSize, max);
                if (splitHorizontally)
                {
                    this.Child1 = new Leaf(X, Y, Width, split, MinLeafSize);
                    this.Child2 = new Leaf(X, Y + split, Width, Height - split, MinLeafSize);
                }
                else
                {
                    this.Child1 = new Leaf(X, Y, split, Height, MinLeafSize);
                    this.Child2 = new Leaf(X + split, Y, Width - split, Height, MinLeafSize);
                }
                return true;
            }
        }

        private enum CellType
        {
            Wall,
            Floor,
            Path,
            Enemy,
        }
    }
}