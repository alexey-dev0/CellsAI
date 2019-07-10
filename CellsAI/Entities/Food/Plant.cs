using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CellsAI.Entities.Food
{
	public class Plant : Eatable
	{
		private static Texture2D _myTexture;
		public Plant(int x, int y, int value = 0) : base(x, y, value)
		{
			if (_myTexture == null) CreateTexture();
			_texture = _myTexture;
		}

		private void CreateTexture()
		{
			var r = new Random();
			var diam = GameParameters.CELL_SIZE;
			_myTexture = new Texture2D(MyGame.SprBatch.GraphicsDevice, diam, diam);
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

			_myTexture.SetData(data);
		}
	}
}
