using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public abstract class Creature : Drawable
	{
		public enum Rotation
		{
			Right,
			Down,
			Left,
			Up
		}

		protected Brain _brain;
		protected List<IReceptor> _receptors;
		protected List<IEffector> _effectors;

		public Rotation MyRotation { get; set; }
		public int Health { get; set; }
		public readonly int MaxHealth = 1000;

		public Creature()
		{
			_receptors = new List<IReceptor>();
			_effectors = new List<IEffector>();
			CreateTexture();
			Health = MaxHealth;
		}

		private void CreateTexture()
		{
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
						if (pos.LengthSquared() <= radsq * 0.7)
							data[ind] = new Color(0xff, 0x26, 0x00);
						else
							data[ind] = Color.Black;
					else
						data[ind] = Color.Transparent;
				}

			for (int i = diam / 2; i < diam; i++)
				data[diam * (int)rad + i] = data[diam * (int)(rad + 1) + i] = Color.White;

			_texture.SetData(data);
		}

		public override void Update()
		{
			Health--;
			if (Health <= 0) return;
			foreach (var receptor in _receptors)
				receptor.Receive();
			_brain.Update();
			foreach (var effector in _effectors)
				effector.Perform();
		}

		public void Move(int dx, int dy)
		{
			MyGame.World[X, Y].Leave(this);
			if (Health <= 0) return;
			X += dx;
			Y += dy;
			MyGame.World[X, Y].Enter(this);
		}

		public override string ToString()
		{
			var result = "";
			foreach (var r in _receptors)
				foreach (var v in r.Values)
					result += $"{v}, ";
			result += "| ";
			foreach (var e in _effectors)
				result += $"{e.Value}, ";
			return result + "\n";
		}
	}
}