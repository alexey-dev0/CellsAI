namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Rotator : IEffector
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
			_creature.MyRotation = (Creature.Rotation)((int)_creature.MyRotation % 4);
		}
	}
}