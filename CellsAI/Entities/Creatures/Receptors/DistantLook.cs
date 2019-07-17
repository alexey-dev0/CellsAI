using CellsAI.Entities.Food;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Receptors
{
	internal class DistantLook : IReceptor
	{
		private const int VISIBILITY = 16;

		public DistantLook(Creature creature) : base(creature)
		{
			Values = new List<double>() { 0, 0 };
		}

		public override void Receive()
		{
			_creature.GetDirection(out int dx, out int dy);

			int vx = _creature.X;
			int vy = _creature.Y;

			Values[0] = Values[1] = 0.0;

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