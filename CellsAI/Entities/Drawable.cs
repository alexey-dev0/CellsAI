using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CellsAI.Entities
{
	public abstract class Drawable : Entity
	{
		protected Texture2D _texture;

		public Texture2D GetTexture()
			=> _texture ?? ErrorTexture();

		protected static Texture2D ErrorTexture()
		{
			var size = GameParameters.CELL_SIZE;
			var result = new Texture2D(MyGame.SprBatch.GraphicsDevice, size, size);
			var data = new Color[size * size];
			for (int i = 0; i < size * size; i++)
				data[i] = i % 2 == 0 ? Color.Purple : Color.Black;
			result.SetData(data);
			return result;
		}
	}
}