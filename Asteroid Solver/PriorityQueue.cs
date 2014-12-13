using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroid_Solver
{
	public class PriorityQueue
	{
		private readonly List<Point> _tiles;

		public PriorityQueue()
		{
			// stores points which represent the index of a tile in the space map.
			_tiles = new List<Point>();
		}

		public void Push(Point next, Space space)
		{
			// prioritize by the cost according to the space.
			for (var i = 0; i < _tiles.Count; i++)
			{
				double currentCost = space.GetTile(_tiles[i].X, _tiles[i].Y).Cost, nextCost = space.GetTile(next.X, next.Y).Cost;
				if (currentCost <= nextCost) continue;
				_tiles.Insert(i, next);
				return;
			}
			_tiles.Add(next);
		}

		public Point Pop()
		{
			var tile = _tiles[0];
			_tiles.RemoveAt(0);
			return tile;
		}

		public bool Empty()
		{
			return _tiles.Count == 0;
		}

		public void Clear()
		{
			_tiles.Clear();
		}
	}
}
