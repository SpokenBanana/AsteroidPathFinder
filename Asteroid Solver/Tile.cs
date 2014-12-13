using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroid_Solver
{
	public class Tile
	{
		public char Content { get;  set; }
		public bool Visited, Path;
		public double Cost;

		public Tile(char content)
		{
			Content = content;
			Cost = 0;
		}

	}
}
