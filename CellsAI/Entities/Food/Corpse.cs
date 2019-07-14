using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CellsAI.Entities.Food
{
	internal class Corpse : Eatable
	{
		public Corpse(int x, int y, int value = 0) : base(x, y, value)
		{
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
							data[ind] = new Color(0xa2, 0x26, 0x18);
						else
							data[ind] = new Color(0x66, 0x66, 0x66);
					else
						data[ind] = Color.Transparent;
				}

			_texture.SetData(data);
		}
	}
}