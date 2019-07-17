using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities
{
	public abstract class Drawable : Entity
	{
		protected Texture2D _texture;

		protected Drawable(int x, int y) : base(x, y)
		{
		}

		private Texture2D Texture
			=> _texture ?? ErrorTexture();

		protected static Texture2D ErrorTexture()
		{
			var size = CELL_SIZE;
			var result = new Texture2D(GAME.SprBatch.GraphicsDevice, size, size);
			var data = new Color[size * size];
			for (int i = 0; i < size * size; i++)
				data[i] = i % 2 == 0 ? Color.Purple : Color.Black;
			result.SetData(data);
			return result;
		}

		public virtual void Draw(Vector2 position)
		{
			GAME.SprBatch.Draw(
					texture: Texture,
					position: position,
					sourceRectangle: null,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: new Vector2(SCALE),
					effects: SpriteEffects.None,
					layerDepth: 0.5f);
		}
	}
}