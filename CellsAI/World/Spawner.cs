using CellsAI.Entities;
using CellsAI.Entities.Creatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeuralNetworkLib;
using System;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.World
{
	public class Spawner : Drawable
	{
		public Color MyColor;
		public readonly List<Creature> _creatures;
		private readonly List<Creature> _temp;
		private int _creatureCount;
		private int _genCount;
		private int _maxLifetime;
		private int _spawnRadius = 8;
		private int _id;
		private Random _rand;
		public Vector2 SpawnPos;

		public Spawner(int x, int y, int startCount, int id) : base(x, y)
		{
			_rand = new Random();
			_id = id;
			SpawnPos = new Vector2(x, y);
			_creatures = new List<Creature>();
			_temp = new List<Creature>();
			MyColor = GetColor();
			if (_myTexture == null) CreateTexture();
			_texture = _myTexture;
			AddCreatures(startCount);
		}

		private Color GetColor()
		{
			var r = _rand.Next(256);
			var g = _rand.Next(256);
			var b = _rand.Next(256);
			var a = 100;
			return new Color(r, g, b, a);
		}

		public override void Update()
		{
			for (int i = 0; i < _creatures.Count; i++)
				_creatures[i].Update();

			for (int i = _creatures.Count - 1; i >= 0; i--)
			{
				if (_creatures[i].Health <= 0)
				{
					_creatures.RemoveAt(i);
					if (_creatures.Count == 3)
						_temp.AddRange(_creatures);
					if (_creatures.Count == 0)
					{
						ResetCreatures();
						break;
					}
				}
			}
		}

		private NeuralNetwork network0;

		private void ResetCreatures()
		{
			_temp.Sort((a, b) => b.Lifetime.CompareTo(a.Lifetime));
			if (_temp[0].Lifetime > _maxLifetime)
			{
				network0 = _temp[0].GetNetwork();
				_maxLifetime = _temp[0].Lifetime;
			}
			var network1 = _temp[0].GetNetwork();
			var network2 = _temp[1].GetNetwork();
			var network3 = _temp[2].GetNetwork();
			_temp.Clear();

			var range = new double[] { 0.1, 0.3, 0.4, 0.5 };
			for (int i = 0; i < _creatureCount; i++)
			{
				double j = (double)i / _creatureCount;
				var network = j < range[0] ? network0
					: j < range[1] ? network1
					: j < range[2] ? network2
					: j < range[3] ? network3
					: null;
				AddCreature(network, 1 + i);
			}
			_genCount++;
		}

		public void AddCreatures(int count)
		{
			_creatureCount = count;
			for (int i = 0; i < _creatureCount; i++)
				AddCreature();
			_genCount = 1;
		}

		private void AddCreature(NeuralNetwork network = null, int changeCount = 0)
		{
			int x = (int)SpawnPos.X;
			int y = (int)SpawnPos.Y;
			int dx = 0, dy = 0;
			while (dx * dx + dy * dy >= _spawnRadius * _spawnRadius
				|| GAME.World[x + dx, y + dy].MyType != Cell.CellType.Ground
				|| GAME.World[x + dx, y + dy].Content.Find(e => e is Creature) != null)
			{
				dx = _rand.Next(-_spawnRadius, _spawnRadius + 1);
				dy = _rand.Next(-_spawnRadius, _spawnRadius + 1);
			}
			var creature = new SimpleCreature(x + dx, y + dy, this, network, changeCount);
			_creatures.Add(creature);
		}

		public void AddNewCreatures(params Creature[] creatures)
			=> _creatures.AddRange(creatures);

		private Texture2D _myTexture;

		protected void CreateTexture()
		{
			var diam = CELL_SIZE;
			_myTexture = new Texture2D(GAME.SprBatch.GraphicsDevice, diam, diam);
			var data = new Color[diam * diam];

			float rad = diam * 0.5f;
			float radsq = rad * rad;

			for (int x = 0; x < diam; x++)
				for (int y = 0; y < diam; y++)
				{
					int ind = x * diam + y;
					var pos = new Vector2(x - rad, y - rad);
					if (pos.LengthSquared() <= radsq)
						if (pos.LengthSquared() <= radsq * 0.93)
							data[ind] = MyColor;
						else
							data[ind] = Color.Black;
					else
						data[ind] = Color.Transparent;
				}

			_myTexture.SetData(data);
		}

		public override void Draw(Vector2 position)
		{
			GAME.SprBatch.Draw(
					texture: _texture,
					position: position,
					sourceRectangle: null,
					color: Color.White,
					rotation: 0f,
					origin: new Vector2(CELL_SIZE * 0.5f),
					scale: new Vector2(SCALE * (_spawnRadius * 2 + 1)),
					effects: SpriteEffects.None,
					layerDepth: 0.7f);
		}

		public override string ToString()
		{
			var result = $"Spawner #{_id}:\n";
			result += $"    Color: {MyColor}\n";
			result += $"    Creatures: {_creatureCount} | {_creatures.Count}\n";
			result += $"    Generation: {_genCount}\n";
			result += $"    Max Lifetime: {_maxLifetime}\n";
			if (_creatures.Count > 0)
				result += $"Top creature:\n{_creatures[0]}";
			return result + '\n';
		}

		public override void Delete()
		{
			foreach (var creature in _creatures)
				creature.Delete();
			base.Delete();
		}
	}
}