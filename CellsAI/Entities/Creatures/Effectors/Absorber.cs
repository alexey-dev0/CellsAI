using CellsAI.Entities.Food;
using CellsAI.Game;

namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Absorber : IEffector
	{
		private Creature _creature;

		public double Value { get; set; }

		public Absorber(Creature creature)
		{
			_creature = creature;
		}

		public void Perform()
		{
			if (Value < 0.5) return;
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

			int vx = _creature.X + dx;
			int vy = _creature.Y + dy;

			var food = MyGame.World[vx, vy].Content.Find(e => e is Eatable);
			if (food != null) _creature.Eat(food as Eatable);
		}
	}
}