using CellsAI.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CellsAI.World
{
	public class Cell
	{
		public enum CellType
		{
			Water,
			Sand,
			Ground,
			Mountains
		}

		public Chunk Parent { get; }

		public List<Entity> Content;

		public void Enter(Entity entity)
		{
			if (!Content.Contains(entity))
			{
				Content.Add(entity);
				if (entity is Drawable) Parent.Enter(entity as Drawable);
			}
		}

		public void Leave(Entity entity)
		{
			if (Content.Contains(entity))
			{
				Content.Remove(entity);
				if (entity is Drawable) Parent.Leave(entity as Drawable);
			}
		}

		// [0..1]
		public double _height;

		public Cell(Chunk parent, double height)
		{
			Parent = parent;
			_height = height;
			//MyType = CellType.Water;
			//Color = Color.Red;
			Content = new List<Entity>();
			ColorFromHeight();
		}

		public Color Color;

		public CellType MyType;

		private void ColorFromHeight()
		{
			_height *= 12;
			_height = (int)_height;
			_height /= 12;

			var water = new double[] { 0.0, 0.3 };
			var sand = new double[] { 0.3, 0.4 };
			var ground = new double[] { 0.4, 0.8 };
			var stone = new double[] { 0.8, 1.0 };

			double[] segment;

			if (_height < water[1])
			{
				segment = water;
				MyType = CellType.Water;
			}
			else if (_height < sand[1])
			{
				segment = sand;
				MyType = CellType.Sand;
			}
			else if (_height < ground[1])
			{
				segment = ground;
				MyType = CellType.Ground;
			}
			else
			{
				segment = stone;
				MyType = CellType.Mountains;
			}

			_height = 1.0 / (segment[1] - segment[0]) * (_height - segment[0]);

			if (segment == water)
				Color = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, (float)_height);
			else if (segment == sand)
				Color = Color.Lerp(Color.Wheat, Color.Tan, (float)_height);
			else if (segment == ground)
				Color = Color.Lerp(Color.YellowGreen, Color.DarkGreen, (float)_height);
			else
				Color = Color.Lerp(new Color(0x54, 0x3D, 0x21), Color.Snow, (float)_height);
		}
	}
}