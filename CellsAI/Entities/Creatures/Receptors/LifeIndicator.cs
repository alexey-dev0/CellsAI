using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	class LifeIndicator : IReceptor
	{
		private Creature _creature;

		public List<double> Values { get; set; }

		public LifeIndicator(Creature creature)
		{
			_creature = creature;
			Values = new List<double>() { 0 };
		}

		public void Receive()
		{
			Values[0] = _creature.Health / (double)_creature.MaxHealth;
		}
	}
}
