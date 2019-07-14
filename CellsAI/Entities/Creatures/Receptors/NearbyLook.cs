using CellsAI.Entities.Food;
using CellsAI.Game;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	class NearbyLook : IReceptor
	{
		private const int VISIBILITY = 4;
		private Creature _creature;

		public List<double> Values { get; }

		public NearbyLook(Creature creature)
		{
			_creature = creature;
			Values = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0 };
		}

		public void Receive()
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
						var cell = MyGame.World[vx + i * k, vy + j * k];
						if (cell.Content.Count > 0 && cell.Content[0] is Eatable)
						{
							Values[(int)Creature.GetRotation(i, j)] = 1.0;
							found = true;

						}
					}
		}
	}
}
