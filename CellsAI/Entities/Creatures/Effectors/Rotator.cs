namespace CellsAI.Entities.Creatures.Effectors
{
	class Rotator : IEffector
	{
		private Creature _creature;

		public double Value { get; set; }

		public Rotator(Creature creature)
		{
			_creature = creature;
		}

		public void Perform()
		{
			if (Value <= 0.1) _creature.MyRotation--;
			else if (Value >= 0.9) _creature.MyRotation++;
		}
	}
}
