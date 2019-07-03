using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CellsAI.Entities
{
	public class Food : Drawable
	{
		// [1..100]
		public int FoodValue { get; }

		public Food(int x, int y, int value = 0)
		{
			X = x;
			Y = y;

			FoodValue = value <= 0 ? new Random(MyGame.Seed).Next(1, 100) : value;
			CreateTexture();
		}

		private void CreateTexture()
		{
			var r = new Random(MyGame.Seed);
			var diam = GameParameters.CELL_SIZE;
			_texture = new Texture2D(MyGame.SprBatch.GraphicsDevice, diam, diam);
			var data = new Color[diam * diam];

			float rad = diam / 2f;
			float radsq = rad * rad;

			for (int x = 0; x < diam; x++)
				for (int y = 0; y < diam; y++)
				{
					int ind = x * diam + y;
					var pos = new Vector2(x - rad, y - rad);
					if (pos.LengthSquared() <= radsq)
						if (r.NextDouble() < 0.7)
							data[ind] = new Color(0x1d, 0x54, 0x00);
						else
							data[ind] = Color.Transparent;
					else
						data[ind] = Color.Transparent;
				}

			_texture.SetData(data);
		}
	}
}