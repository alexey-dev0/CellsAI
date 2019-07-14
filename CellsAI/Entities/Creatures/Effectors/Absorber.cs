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
			if (Value < 0.1) return;

			int dx, dy;
			_creature.GetDirection(out dx, out dy);

			int vx = _creature.X + dx;
			int vy = _creature.Y + dy;

			var food = MyGame.World[vx, vy].Content.Find(e => e is Eatable);
			if (food != null) _creature.Eat(food as Eatable);
		}
	}
}