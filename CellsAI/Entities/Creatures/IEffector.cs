namespace CellsAI.Entities.Creatures
{
	public abstract class IEffector
	{
		protected readonly Creature _creature;

		public double Value { get; set; }

		protected IEffector(Creature creature)
		{
			_creature = creature;
		}

		public abstract void Perform();
	}
}