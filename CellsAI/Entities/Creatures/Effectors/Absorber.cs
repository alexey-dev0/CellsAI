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
			var x = _creature.X;
			var y = _creature.Y;
			var cell = MyGame.World[x, y];
			var food = cell.Content.Find(e => e is Food);
			if (food == null) return;
			_creature.Health += (food as Food).FoodValue;
			cell.Leave(food);
		}
	}
}