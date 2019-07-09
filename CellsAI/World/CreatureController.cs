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
		public static List<Creature> _creatures;
		private readonly List<Creature> _temp;
		private int _creatureCount;
		private int _genCount;

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

			//if (_creatures.Count > 0)
			//Game.DebugInfo.DebugMessage += _creatures[0].ToString();
			Game.DebugInfo.DebugMessage += $"CREATURES: {_creatureCount} | {_creatures.Count}\n";
			Game.DebugInfo.DebugMessage += $"Generation: {_genCount}\n";
		}

		private void ResetCreatures()
		{
			var position = Vector2.Zero;
			var r = new Random();

			if (_temp.Count < 3) throw new Exception();

			_temp.Sort((a, b) => b.Lifetime.CompareTo(a.Lifetime));
			var network1 = _temp[0].GetNetwork();
			var network2 = _temp[1].GetNetwork();
			var network3 = _temp[2].GetNetwork();
			_temp.Clear();

			var range = new double[] { 0.5, 0.8, 0.9 };
			for (int i = 0; i < _creatureCount; i++)
			{
				double j = (double)i / _creatureCount;
				var network = j < range[0] ? network1 : j < range[1] ? network2 : j < range[2] ? network3 : null;
				position = AddCreature(position, r, network, i);
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
			int x = (int)pos.X;
			int y = (int)pos.Y;
			while (MyGame.World[x, y].MyType == Cell.CellType.Water
				|| MyGame.World[x, y].Content.Count > 0)
			{
				x += -20 + r.Next(41);
				y += -20 + r.Next(41);
			}
			var creature = new SimpleCreature(x, y, network, changeCount);
			MyGame.World[x, y].Enter(creature);
			_creatures.Add(creature);
			return new Vector2(x, y);
		}
	}
}