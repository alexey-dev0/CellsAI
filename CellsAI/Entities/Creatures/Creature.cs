using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public abstract class Creature : Drawable
	{
		protected Brain _brain;
		protected SpriteBatch _sprBatch;
		protected List<IReceptor> _receptors;
		protected List<IEffector> _effectors;

		// Consider how to create network
		public Creature(SpriteBatch sprBatch)
		{
			_receptors = new List<IReceptor>();
			_effectors = new List<IEffector>();
			_sprBatch = sprBatch;
			CreateTexture();
		}

		private void CreateTexture()
		{
			var diam = GameConstants.CELL_SIZE;
			_texture = new Texture2D(_sprBatch.GraphicsDevice, diam, diam);
			var data = new Color[diam * diam];

			float rad = diam / 2f;
			float radsq = rad * rad;

			for (int x = 0; x < diam; x++)
				for (int y = 0; y < diam; y++)
				{
					int ind = x * diam + y;
					var pos = new Vector2(x - rad, y - rad);
					if (pos.LengthSquared() <= radsq)
						data[ind] = Color.Black;
					else
						data[ind] = Color.Transparent;
				}

			_texture.SetData(data);
		}

		public void Update()
		{
			foreach (var receptor in _receptors)
				receptor.Receive();
			_brain.Update();
			foreach (var effector in _effectors)
				effector.Perform();
		}
	}
}