using CellsAI.Entities.Food;
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
			Values = new List<double>() { 0, 0, 0, 0, 0, 0 };
			//for (int i = 0; i < VISIBILITY * 2; i++)
			//	Values.Add(0);
		}

		public void Receive()
		{
			int dx, dy;
			_creature.GetDirection(out dx, out dy);
			int cx = _creature.X;
			int cy = _creature.Y;

			Values[0] = (int)MyGame.World[cx + dx, cy + dy].MyType / 3.0;//MyGame.World[cx + dx, cy + dy].MyType == World.Cell.CellType.Water ? 1.0 : 0.0;
			Values[1] = Values[2] = Values[3] = Values[4] = Values[5] = 0.0;

			for (int i = 0; i < VISIBILITY * 2; i++)
			{
				var cell = MyGame.World[cx + dx * i, cy + dy * i];
				if (cell.Content.Count > 0 && cell.Content[0] is Eatable)
				{
					Values[1] = 1.0;
					break;
				}
			}
			if (Values[1] == 1.0) return;

			for (int i = -2; i <= 2; i++)
			{
				if (i == 0) continue;
				dx = i % 2;
				dy = i / 2;
				int vx = cx;
				int vy = cy;
				for (int j = 0; j < VISIBILITY; j++)
				{
					vx += dx;
					vy += dy;
					var cell = MyGame.World[vx, vy];
					if (cell.Content.Count > 0 && cell.Content[0] is Eatable)
					{
						Values[i < 0 ? i + 2 : i + 1] = 1.0;// cell.Content[0] is Creature ? 1.0 : 0.5;
															//Values[2] = (double)i / VISIBILITY;
						break;
					}
				}
				if (Values[i < 0 ? i + 2 : i + 1] == 1.0) break;
				//Values[i * 2] = (int)cell.MyType / 3.0;
				//if (cell.Content.Count > 0)
				//	Values[i * 2 + 1] = cell.Content[0] is Creature ? 1.0 : 0.5;
				//else
				//	Values[i * 2 + 1] = 0.0;
			}
		}
	}
}