using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public abstract class IReceptor
	{
		protected readonly Creature _creature;
		public List<double> Values { get; protected set; }

		protected IReceptor(Creature creature)
		{
			_creature = creature;
		}

		public abstract void Receive();
	}
}