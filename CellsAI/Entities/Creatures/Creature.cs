using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeuralNetworkLib;
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
		protected bool _deleted;

		public Rotation MyRotation { get; set; }
		public int Lifetime;

		protected int _health;
		public int Health
		{
			get { return _health; }
			set
			{
				if (_deleted) return;
				_health = value > MaxHealth ? MaxHealth : value;
				if (_health <= 0) Delete(); 
			}
		}

		public readonly int MaxHealth = 60;

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
			if (_deleted) return;
			Health--;
			Lifetime++;
			foreach (var receptor in _receptors)
				receptor.Receive();
			_brain.Update();
			foreach (var effector in _effectors)
				effector.Perform();
		}

		private void Delete()
		{
			MyGame.World[X, Y].Leave(this);
			_deleted = true;
		}

		public void Move(int dx, int dy)
		{
			if (_deleted) return;
			MyGame.World[X, Y].Leave(this);
			X += dx;
			Y += dy;
			// TODO
			if (MyGame.World[X, Y].MyType == World.Cell.CellType.Water)
				Health = 0;
			else
				MyGame.World[X, Y].Enter(this);
			var food = MyGame.World[X, Y].Content.Find(e => e is Food);
			if (food != null)
			{
				Health += (food as Food).FoodValue;
				MyGame.World[X, Y].Leave(food);
			}
		}

		public NeuralNetwork GetNetwork()
			=> _brain.GetNetwork();

		public override string ToString()
		{
			if (_deleted) return "DELETED\n";
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