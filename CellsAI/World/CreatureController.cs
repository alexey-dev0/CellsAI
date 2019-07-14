using CellsAI.Entities.Creatures;
using CellsAI.Game;
using Microsoft.Xna.Framework;
using NeuralNetworkLib;
using System;
using System.Collections.Generic;

namespace CellsAI.World
{
	public class CreatureController
	{
		public bool CanDebug;
		public static List<Creature> _creatures;
		private readonly List<Creature> _temp;
		private int _creatureCount;
		private int _genCount;
		private int _maxLifetime;
		private int _avgLifetime;

		public CreatureController()
		{
			_creatures = new List<Creature>();
			_temp = new List<Creature>();
		}

		public void Update()
		{
			foreach (var creature in _creatures)
				creature.Update();

			for (int i = _creatures.Count - 1; i >= 0; i--)
			{
				if (_creatures[i].Health <= 0)
				{
					_avgLifetime *= _creatureCount - _creatures.Count;
					_avgLifetime += _creatures[i].Lifetime;
					_creatures.RemoveAt(i);
					_avgLifetime /= _creatureCount - _creatures.Count;
					if (_creatures.Count == 3)
						_temp.AddRange(_creatures);
					if (_creatures.Count == 0)
					{
						_avgLifetime = 0;
						ResetCreatures();
						break;
					}
				}
			}

			if (CanDebug)
			{
				Game.DebugInfo.DebugMessage += $"CREATURES: {_creatureCount} | {_creatures.Count}\n";
				Game.DebugInfo.DebugMessage += $"Generation: {_genCount}\n";
				Game.DebugInfo.DebugMessage += $"Max Lifetime: {_maxLifetime}\n";
				Game.DebugInfo.DebugMessage += $"Avg Lifetime: {_avgLifetime}\n";
				//if (network0 != null)
				//	Game.DebugInfo.DebugMessage += (network0 as SimpleNetwork).GetNeuroPresentation(false);

				if (_creatures.Count > 0)
					Game.DebugInfo.DebugMessage += _creatures[0].ToString();
				CanDebug = false;
			}
		}

		private NeuralNetwork network0;

		private void ResetCreatures()
		{
			var position = Vector2.Zero;
			var r = new Random();

			//if (_temp.Count < 3) throw new Exception();

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
				position = AddCreature(position, r, network, 1 + i * 10);
			}
			_genCount++;
		}

		public void AddCreatures(int count)
		{
			_creatureCount = count;
			var position = Vector2.Zero;
			var r = new Random(MyGame.Seed);
			for (int i = 0; i < _creatureCount; i++)
				position = AddCreature(position, r);
			_genCount = 1;
		}

		private Vector2 AddCreature(Vector2 pos, Random r, NeuralNetwork network = null, int changeCount = 0)
		{
			//int counterBug = 0;
			int x = (int)pos.X;
			int y = (int)pos.Y;
			while (MyGame.World[x, y].MyType != Cell.CellType.Ground
				|| MyGame.World[x, y].Content.Count > 0)
			{
				//counterBug++;
				x += -40 + r.Next(81);
				y += -40 + r.Next(81);
				// if Island
				//if (counterBug > 100)
				//{
				//	x = 0;
				//	y = 0;
				//	counterBug = 0;
				//}
			}
			var creature = new SimpleCreature(x, y, network, changeCount);
			MyGame.World[x, y].Enter(creature);
			_creatures.Add(creature);
			return new Vector2(x, y);
		}
	}
}