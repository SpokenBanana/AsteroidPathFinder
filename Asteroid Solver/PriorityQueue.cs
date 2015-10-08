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

		// binary insert into the array
		public void Push(Point next, Space space)
		{
			if (_tiles.Count == 0) _tiles.Add(next);
			var lower = 0;
			var upper = _tiles.Count;
			var current = (lower + upper)/2;
			double currentCost;
			var nextCost = space.GetTile(next.X, next.Y).Cost;
			while (lower < upper)
			{
				current = (lower + upper)/2;
				currentCost = space.GetTile(_tiles[current].X, _tiles[current].Y).Cost;
				if (currentCost == nextCost)
				{
					_tiles.Insert(current, next);
					return;
				}
				if (nextCost < currentCost)
					upper = current - 1;
				else if (nextCost > currentCost)
					lower = current + 1;
			}
			currentCost = space.GetTile(_tiles[current].X, _tiles[current].Y).Cost;
			if (nextCost <= currentCost)
				_tiles.Insert(current, next);
			else
				_tiles.Insert(current+1, next);

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
