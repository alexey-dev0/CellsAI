namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Mover : IEffector
	{
		private Creature _creature;

		public double Value { get; set; }

		public Mover(Creature creature)
		{
			_creature = creature;
		}

		public void Perform()
		{
			if (Value < 0.6) return;
			int dx, dy;
			_creature.GetDirection(out dx, out dy);
			_creature.Move(dx, dy);
		}
	}
}