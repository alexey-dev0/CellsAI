using CellsAI.Game;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	public class StraightLook : IReceptor
	{
		private const int VISIBILITY = 10;

		public List<double> Values { get; }

		private Creature _creature;

		public StraightLook(Creature creature)
		{
			_creature = creature;
			Values = new List<double>() { 0, 0, 0 };
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

			Values[0] = (int)MyGame.World[vx + dx, vy + dy].MyType / 4.0;
			double type = 0.0;
			double dist = 0.0;

			for (int i = 0; i < VISIBILITY; i++)
			{
				vx += dx;
				vy += dy;
				if (MyGame.World[vx, vy].Content.Count > 0)
				{
					type = MyGame.World[vx, vy].Content[0] is Creature ? 1.0 : 0.5;
					dist = 1.0 / VISIBILITY * i;
					break;
				}
			}
			Values[1] = type;
			Values[2] = dist;
		}
	}
}