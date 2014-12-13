using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroid_Solver
{
	public class PathPainter
	{
		private class Button
		{
			public Rectangle Position;
			readonly Texture2D _buttonTexture;
			readonly String _text;
			readonly SpriteFont _font;

			public Button(ContentManager content, Rectangle position, String text)
			{
				Position = position;
				_text = text;
				_font = content.Load<SpriteFont>("SpriteFont1");
				_buttonTexture = content.Load<Texture2D>("button");
			}

			public void Draw(SpriteBatch context, Color color)
			{
				context.Draw(_buttonTexture, Position, color);
				context.DrawString(_font, _text, new Vector2(Position.X + 10, Position.Y + 10), Color.White);
			}
		}

		private readonly Button _asteroid, _gravity, _erase;
		public char Paint;

		public PathPainter(ContentManager content, Space space)
		{
			Paint = Space.Asteroid;
			_asteroid = new Button(content, new Rectangle(space.Size()*PathSolver.CellSize + 20, 100, 150, 40), "Paint Asteroids");
			_gravity = new Button(content, new Rectangle(space.Size()*PathSolver.CellSize + 20, 250, 150, 40), "Paint Gravity Wells");
			_erase = new Button(content, new Rectangle(space.Size()*PathSolver.CellSize + 20, 400, 150, 40), "Erase");
		}

		public void Update()
		{
			var mouse = Mouse.GetState();
			if (_asteroid.Position.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
				Paint = Space.Asteroid;
			if (_gravity.Position.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
				Paint = Space.Gravity;
			if (_erase.Position.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
				Paint = Space.Empty;
		}

		public void Draw(SpriteBatch context)
		{
			_asteroid.Draw(context, Paint == Space.Asteroid ? Color.DeepSkyBlue : Color.White);
			_gravity.Draw(context, Paint == Space.Gravity ? Color.DeepSkyBlue :Color.White);
			_erase.Draw(context, Paint == Space.Empty ? Color.DeepSkyBlue : Color.White);
		}
	}
}
