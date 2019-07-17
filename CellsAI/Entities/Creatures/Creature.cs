using CellsAI.Entities.Food;
using CellsAI.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeuralNetworkLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures
{
	public abstract class Creature : Drawable
	{
		protected Brain _brain;
		protected List<IReceptor> _receptors;
		protected List<IEffector> _effectors;
		public Spawner MySpawner;

		public Rotation MyRotation { get; set; }

		public Vector2 GetDirection()
		{
			switch (MyRotation)
			{
				case Rotation.R:
					return new Vector2(1, 0);

				case Rotation.RD:
					return new Vector2(1, 1);

				case Rotation.D:
					return new Vector2(0, 1);

				case Rotation.LD:
					return new Vector2(-1, 1);

				case Rotation.L:
					return new Vector2(-1, 0);

				case Rotation.LU:
					return new Vector2(-1, -1);

				case Rotation.U:
					return new Vector2(0, -1);

				case Rotation.RU:
					return new Vector2(1, -1);

				default:
					return Vector2.Zero;
			}
		}

		public static Rotation GetRotation(int x, int y)
		{
			if (x == 1)
			{
				if (y == 1) return Rotation.RD;
				else if (y == -1) return Rotation.RU;
				else return Rotation.R;
			}
			else if (x == -1)
			{
				if (y == 1) return Rotation.LD;
				else if (y == -1) return Rotation.LU;
				else return Rotation.L;
			}
			else
			{
				if (y == 1) return Rotation.D;
				else return Rotation.U;
			}
		}

		public void GetDirection(out int x, out int y)
		{
			GetDirection().Deconstruct(out float xx, out float yy);
			x = (int)xx;
			y = (int)yy;
		}

		public int Lifetime;

		protected int _health;

		public int Health
		{
			get { return _health; }
			set
			{
				if (IsDeleted) return;
				_health = value > MaxHealth ? MaxHealth : value;
				if (_health <= 0) Delete();
			}
		}

		public readonly int MaxHealth = 300;

		protected Texture2D _myTexture;

		public Creature(int x, int y, Spawner spawner)
			: base(x, y)
		{
			MySpawner = spawner;
			_receptors = new List<IReceptor>();
			_effectors = new List<IEffector>();
			MyColor = new Color(spawner.MyColor, 200);
			if (_myTexture == null) CreateTexture();
			_texture = _myTexture;
			Health = MaxHealth;
			MyRotation = (Rotation)new Random().Next(4);
		}

		public void Eat(Eatable food)
		{
			Health += food.FoodValue;
			food.Delete();
		}

		public Color MyColor = new Color(0xff, 0x26, 0x00);

		protected void CreateTexture()
		{
			var diam = CELL_SIZE;
			_myTexture = new Texture2D(GAME.SprBatch.GraphicsDevice, diam, diam);
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
							data[ind] = MyColor;
						else
							data[ind] = Color.Black;
					else
						data[ind] = Color.Transparent;
				}

			for (int i = diam / 2; i < diam; i++)
				data[diam * (int)rad + i] = data[diam * (int)(rad + 1) + i] = Color.White;

			_myTexture.SetData(data);
		}

		public override void Draw(Vector2 position)
		{
			var offset = new Vector2(CELL_SIZE * 0.5f);
			var rot = MathHelper.PiOver4 * (int)MyRotation;
			GAME.SprBatch.Draw(
					texture: _texture,
					position: position + offset * SCALE,
					sourceRectangle: null,
					color: Color.White,
					rotation: rot,
					origin: offset,
					scale: new Vector2(SCALE),
					effects: SpriteEffects.None,
					layerDepth: 0.6f);
		}

		public override void Update()
		{
			Health -= 3;
			if (IsDeleted) return;
			Lifetime++;
			foreach (var receptor in _receptors)
			{
				if (IsDeleted) return;
				receptor.Receive();
			}
			_brain.Update();
			foreach (var effector in _effectors)
			{
				if (IsDeleted) return;
				effector.Perform();
			}
		}

		public override void Delete()
		{
			//var cell = GAME.World[X, Y];
			//if (!cell.Content.Exists(e => e is Corpse))
			//	cell.Enter(new Corpse(X, Y));
			base.Delete();
			_myTexture.Dispose();
		}

		public void Move(int dx, int dy)
		{
			Health -= dx * dy == 0 ? 2 : 3;
			if (IsDeleted) return;
			X += dx;
			Y += dy;
			if (GAME.World[X, Y].MyType == World.Cell.CellType.Water)
				Health -= 10;
		}

		public void Attack(Creature other)
		{
			int damage = 50;
			other.Health -= damage;
			Health += damage;
			if (other.Health <= 0) Debug.WriteLine("Assault happens!");
		}

		public NeuralNetwork GetNetwork()
			=> _brain.GetNetwork();

		public override string ToString()
		{
			var result = $"Creature [{GetNetwork().Id}]:\n";
			if (IsDeleted)
			{
				result += "    DELETED";
				return result;
			}
			result += (GetNetwork() as SimpleNetwork).GetNeuroPresentation(true);
			result += $"    Neurons: {GetNetwork().NeuronCount()} \n";
			result += $"    Rotation: {MyRotation}\n";
			result += $"    Health: {Health}\n";
			result += $"    Color: {MyColor}\n";
			result += $"    Lifetime: {Lifetime}\n";
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