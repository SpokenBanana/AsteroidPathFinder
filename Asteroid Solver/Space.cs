using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Microsoft.Xna.Framework;

namespace Asteroid_Solver
{
	public class Space
	{
		public  const char Asteroid = 'A',
			Empty = 'E',
			Gravity = 'G',
			Well = 'X';

		public Point[] Increments =  {
			new Point(-1, -1), new Point(0, -1), new Point(1, -1),
			new Point(-1, 0),					new Point(1, 0),
			new Point(-1, 1), new Point(0, 1), new Point(1, 1)
		};

		public Tile[,] Tiles { private set; get; }

		public Space(int size)
		{
			Tiles = new Tile[size,size];
			for (var i = 0; i < size; i++)
				for (var j = 0; j < size; j++)
					Tiles[i, j] = new Tile(Empty);
		}

		public Tile GetTile(int x, int y)
		{
			return Tiles[x, y];
		}

		public int Size()
		{
			return Tiles.GetLength(0);
		}

		public double Distance(int x1, int y1, int x2, int y2)
		{
			return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
		}

		public void ChangeTile(int x, int y, Tile tile)
		{
			Tiles[x, y].Content = tile.Content;
			if (tile.Content != Gravity) return;

			// gravity cells create little gravity wells around them
			foreach (var point in Increments)
			{
				int newx = x + point.X, newy = y + point.Y;
				if (newx >= 0 && newy >= 0 && newx < Tiles.GetLength(0) && newy < Tiles.GetLength(0) && Tiles[newx, newy].Content == Empty)
					Tiles[newx, newy].Content = Well;
			}
		}

		public List<Point> GetNeighbors(Point location)
		{
			var neighbors = new List<Point>();
			foreach (var incremenet in Increments)
			{
				int newx = location.X + incremenet.X, newy = location.Y + incremenet.Y;
				if (newx >= 0 && newy >= 0 && newx < Tiles.GetLength(0) && newy < Tiles.GetLength(0))
					neighbors.Add(new Point(newx, newy));
			}
			return neighbors;
		}

		public void Reset()
		{
			foreach (var tile in Tiles)
			{
				tile.Visited = false;
				tile.Cost = 0;
			}
		}
	}
}
