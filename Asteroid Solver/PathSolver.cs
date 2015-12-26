using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization.Advanced;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroid_Solver
{
    public class PathSolver
    {
		private readonly Space _space;
	    private readonly List<Point> _path;
	    private Point  _start, _stop;
	    private readonly Texture2D _blank;

	    private readonly Dictionary<Point, Point> _cameFrom;
	    private readonly Dictionary<Point, double> _costSoFar;
	    private readonly PriorityQueue _paths;
	    private readonly PathPainter _painter;

	    public const int CellSize = 12;

		public PathSolver(ContentManager content)
		{
			_path = new List<Point>();
			_space = new Space(50);
			GenerateSpace();
			
			_blank = content.Load<Texture2D>("blank");

		    _cameFrom = new Dictionary<Point, Point>();
		    _costSoFar = new Dictionary<Point, double>();
		    _paths = new PriorityQueue();
		    _costSoFar[_start] = 0;
			_paths.Push(_start, _space);
			_painter = new PathPainter(content, _space);
		}

		public void Update()
		{
			if (!_paths.Empty())
			{
				// get the next logical path
				var current = _paths.Pop();
			    _space.GetTile(current.X, current.Y).Visited = true;
				if (current.Equals(_stop))
				{
					// we made it
					ConstructPath();
					_paths.Clear();
					return;
				}
				// check its neighbors and how far they are
				foreach (var neighbor in _space.GetNeighbors(current))
				{
					double newCost = _costSoFar[current] + _space.Distance(neighbor.X, neighbor.Y, current.X, current.Y);;
					if ((!_costSoFar.ContainsKey(neighbor) || newCost < _costSoFar[neighbor]) && _space.GetTile(neighbor.X, neighbor.Y).Content == Space.Empty)
				    {
					    _costSoFar[neighbor] = newCost;
						_space.GetTile(neighbor.X, neighbor.Y).Visited = true;
						// update costs and add to explore next
					    _space.GetTile(neighbor.X, neighbor.Y).Cost = newCost + _space.Distance(neighbor.X, neighbor.Y, _stop.X, _stop.Y); // add cost of the goal
						_paths.Push(neighbor, _space);
					    _cameFrom[neighbor] = current;
					}
				}
			}
			_painter.Update();
			MouseState mouse = Mouse.GetState();
			
			// user is editting the map
			if (mouse.LeftButton == ButtonState.Pressed)
			{
				var mousePos = new Point(mouse.X/CellSize, mouse.Y/CellSize);
				// make sure it is a valid edit
				if (mouse.X < _space.Size()*CellSize && mouse.X >= 0 && mouse.Y >= 0 && mouse.Y < _space.Size() * CellSize &&
					!mousePos.Equals(_start) && !mousePos.Equals(_stop))
				{
					// a gravity cell will create wells around it, if it creates a well under _stop then a path will never be found
					if (_painter.Paint == Space.Gravity && _space.GetNeighbors(mousePos).Any(neighbor => neighbor.Equals(_stop)))
						return;
					_space.ChangeTile(mousePos.X, mousePos.Y, new Tile(_painter.Paint));
					Reset();
				}
			}
		}

		public void Draw(SpriteBatch context)
		{
			_painter.Draw(context);
			for (var i = 0; i < _space.Size(); i++)
			{
				for (var j = 0; j < _space.Size(); j++)
				{
					Color color = Color.White;
					switch (_space.GetTile(i, j).Content)
					{
						case Space.Gravity:
							color = Color.Yellow;
							break;
						case Space.Well:
							color = Color.Orange;
							break;
						case Space.Asteroid:
							color = Color.Gray;
							break;
					}
					var pathPoint = new Point(i, j);

					context.Draw(_blank, new Rectangle(i * CellSize, j * CellSize, CellSize, CellSize), color);

					// draw visited locations
					if (_space.GetTile(pathPoint.X, pathPoint.Y).Visited)
					{
						color = Color.CornflowerBlue;
						context.Draw(_blank, new Rectangle(i * CellSize, j * CellSize, CellSize, CellSize), color);
					}

					// draw path
					if (_path.Any(path => path.Equals(pathPoint)))
					{
						color = Color.Green;
						context.Draw(_blank, new Rectangle(i * CellSize, j * CellSize, CellSize, CellSize), color);
					}

					// draw start and end points
					if (pathPoint.Equals(_start) || pathPoint.Equals(_stop))
					{
						color = Color.Blue;
						context.Draw(_blank, new Rectangle(i * CellSize, j * CellSize, CellSize, CellSize), color);
					}
				}
			}
		}

		// generate a random map
	    public void GenerateSpace()
	    {
		    int asteroidAmount = (int) ((_space.Size()*_space.Size())*0.15);
		    int gravityAmount = (int) ((_space.Size()*_space.Size())*0.05);
			var random = new Random();
		    int newx = 0, newy = 0;
		    for (var i = 0; i < gravityAmount; i++)
		    {
				do
				{
					newx = random.Next(_space.Size());
					newy = random.Next(_space.Size());
				} while (_space.GetTile(newx, newy).Content != Space.Empty);
				_space.ChangeTile(newx, newy,  new Tile(Space.Gravity));
		    }

		    for (var i = 0; i < asteroidAmount; i++)
		    {
				do
				{
					newx = random.Next(_space.Size());
					newy = random.Next(_space.Size());
				} while (_space.GetTile(newx, newy).Content == Space.Gravity);
				_space.ChangeTile(newx, newy,  new Tile(Space.Asteroid));
		    }
			do
			{
				newx = random.Next(_space.Size());
				newy = random.Next(_space.Size());
			} while (_space.GetTile(newx, newy).Content != Space.Empty);
			_start = new Point(newx, newy);
			do
			{
				newx = random.Next(_space.Size());
				newy = random.Next(_space.Size());
			} while (_space.GetTile(newx, newy).Content != Space.Empty);
			_stop = new Point(newx, newy);
	    }

	    public void ConstructPath()
	    {
		    var step = _stop;
		    _path.Add(step);
		    while (!_start.Equals(step))
		    {
			    step = _cameFrom[step];
			    _path.Add(step);
		    }	    
	    }

	    public void Reset()
	    {
			_space.Reset();
			_cameFrom.Clear();
			_costSoFar.Clear();
			_paths.Clear();

			_paths.Push(_start, _space);
		    _costSoFar[_start] = 0;
			_path.Clear();
	    }
    }
}
