using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CellsAI.Entities
{
	public abstract class Drawable : Entity
	{
		protected Texture2D _texture;

		public void Draw(SpriteBatch sprBatch, int drawX, int drawY)
		{
			sprBatch.Begin(samplerState: SamplerState.PointClamp);
			sprBatch.Draw(_texture ?? ErrorTexture(sprBatch.GraphicsDevice),
				new Vector2(drawX + X, drawY + Y), Color.White);
			sprBatch.End();
		}

		protected static Texture2D ErrorTexture(GraphicsDevice graphics)
		{
			var size = GameParameters.CELL_SIZE;
			var result = new Texture2D(graphics, size, size);
			var data = new Color[size * size];
			for (int i = 0; i < size * size; i++)
				data[i] = i % 2 == 0 ? Color.Purple : Color.Black;
			result.SetData(data);
			return result;
		}
	}
}