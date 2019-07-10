using CellsAI.Entities.Food;
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

		public readonly int MaxHealth = 30;

		protected static Texture2D _myTexture;

		public Creature()
		{
			_receptors = new List<IReceptor>();
			_effectors = new List<IEffector>();
			if (_myTexture == null) CreateTexture();
			_texture = _myTexture;
			Health = MaxHealth;
		}

		public void Eat(Eatable food)
		{
			Health += food.FoodValue;
			food.Delete();
		}

		private void CreateTexture()
		{
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
						if (pos.LengthSquared() <= radsq * 0.7)
							data[ind] = new Color(0xff, 0x26, 0x00);
						else
							data[ind] = Color.Black;
					else
						data[ind] = Color.Transparent;
				}

			for (int i = diam / 2; i < diam; i++)
				data[diam * (int)rad + i] = data[diam * (int)(rad + 1) + i] = Color.White;

			_myTexture.SetData(data);
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
			//MyGame.World[X, Y].Enter(new Corpse(X, Y));
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
			//var food = MyGame.World[X, Y].Content.Find(e => e is Eatable);
			//if (food != null) Eat(food as Eatable);
		}

		public NeuralNetwork GetNetwork()
			=> _brain.GetNetwork();

		public override string ToString()
		{
			var result = "\nCreature info:\n";
			if (_deleted)
			{
				result += "DELETED";
				return result;
			}
			result += $"Neurons: {GetNetwork().NeuronCount()} \n";
			result += $"Rotation: {MyRotation}\n"; 
			result += "Receptors:\n";
			foreach (var r in _receptors)
			{
				result += $"    {r.GetType().Name}: ";
				foreach (var v in r.Values)
					result += $"{v:f1} ";
				result += "\n";
			}
			result += "Effectors:\n";
			foreach (var e in _effectors)
				result += $"    {e.GetType().Name}: {e.Value:f1}\n";
			return result;
		}
	}
}