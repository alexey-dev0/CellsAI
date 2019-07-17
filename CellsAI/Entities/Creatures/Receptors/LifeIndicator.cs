using System.Collections.Generic;

namespace CellsAI.Entities.Creatures.Receptors
{
	internal class LifeIndicator : IReceptor
	{
		public LifeIndicator(Creature creature) : base(creature)
		{
			Values = new List<double>() { 0 };
		}

		public override void Receive()
		{
			Values[0] = _creature.Health / (double)_creature.MaxHealth;
		}
	}
}