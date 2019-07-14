using CellsAI.Entities.Food;
using CellsAI.Game;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	public class StraightLook : IReceptor
	{
		private const int VISIBILITY = 8;
		private Creature _creature;

		public List<double> Values { get; }

		public StraightLook(Creature creature)
		{
			_creature = creature;
			Values = new List<double>() { 0, 0, 0, 0, 0 };
		}

		public void Receive()
		{
			int dx, dy;
			_creature.GetDirection(out dx, out dy);

			int vx = _creature.X;
			int vy = _creature.Y;

			Values[0] = Values[1] = Values[2] = Values[3] = Values[4] = 0.0;
			Values[1 + (int)MyGame.World[vx + dx, vy + dy].MyType] = 1.0;

			for (int i = 0; i < VISIBILITY; i++)
			{
				vx += dx;
				vy += dy;
				var cell = MyGame.World[vx, vy];
				if (cell.Content.Count > 0 && cell.Content[0] is Eatable)
				{
					Values[0] = 1.0;
					break;
				}
			}
		}
	}
}