using CellsAI.Game;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	public class StraightLook : IReceptor
	{
		private const int VISIBILITY = 8;

		public List<double> Values { get; }

		private Creature _creature;

		public StraightLook(Creature creature)
		{
			_creature = creature;
			Values = new List<double>();
			for (int i = 0; i < VISIBILITY * 2; i++)
				Values.Add(0);
		}

		public void Receive()
		{
			int dx = 0, dy = 0;
			switch (_creature.MyRotation)
			{
				case Creature.Rotation.Right:
					dx = 1;
					break;

				case Creature.Rotation.Down:
					dy = 1;
					break;

				case Creature.Rotation.Left:
					dx = -1;
					break;

				case Creature.Rotation.Up:
					dy = -1;
					break;
			}

			int vx = _creature.X;
			int vy = _creature.Y;

			for (int i = 0; i < VISIBILITY; i++)
			{
				vx += dx;
				vy += dy;
				var cell = MyGame.World[vx, vy];
				Values[i * 2] = (int)cell.MyType / 3.0;
				if (cell.Content.Count > 0)
					Values[i * 2 + 1] = cell.Content[0] is Creature ? 1.0 : 0.5;
				else
					Values[i * 2 + 1] = 0.0;
			}
		}
	}
}