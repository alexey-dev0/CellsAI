using CellsAI.Entities.Food;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Receptors
{
	internal class NearbyLook : IReceptor
	{
		private const int VISIBILITY = 6;

		public NearbyLook(Creature creature) : base(creature)
		{
			Values = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0 };
		}

		public override void Receive()
		{
			for (int i = 0; i < 8; i++) Values[i] = 0;
			int vx = _creature.X;
			int vy = _creature.Y;
			bool found = false;

			for (int k = 1; k <= VISIBILITY && !found; k++)
				for (int i = -1; i <= 1 && !found; i++)
					for (int j = -1; j <= 1 && !found; j++)
					{
						if (i == 0 && j == 0) continue;
						var cell = GAME.World[vx + i * k, vy + j * k];
						if (cell.Content.Count > 0 && cell.Content[0] is Eatable)
						{
							Values[(int)Creature.GetRotation(i, j)] = 1.0;
							found = true;
						}
					}
		}
	}
}