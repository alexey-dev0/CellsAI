﻿using CellsAI.Entities.Food;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Receptors
{
	public class StraightLook : IReceptor
	{
		private const int VISIBILITY = 6;

		public StraightLook(Creature creature) : base(creature)
		{
			Values = new List<double>() { 0, 0, 0, 0, 0, 0 };
		}

		public override void Receive()
		{
			_creature.GetDirection(out int dx, out int dy);

			int vx = _creature.X;
			int vy = _creature.Y;

			Values[0] = Values[1] = Values[2] = Values[3] = Values[4] = Values[5] = 0.0;
			Values[2 + (int)GAME.World[vx + dx, vy + dy].MyType] = 1.0;

			for (int i = 0; i < VISIBILITY; i++)
			{
				vx += dx;
				vy += dy;
				var cell = GAME.World[vx, vy];
				if (cell.Content.Count > 0)
				{
					if (cell.Content[0] is Eatable)
						Values[0] = 1.0;
					else
						Values[1] = 1.0;
					break;
				}
			}
		}
	}
}