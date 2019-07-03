using CellsAI.Entities.Creatures.Effectors;
using CellsAI.Entities.Creatures.Receptors;
using NeuralNetworkLib;

namespace CellsAI.Entities.Creatures
{
	internal class SimpleCreature : Creature
	{
		public SimpleCreature(int x, int y) : base()
		{
			_receptors.Add(new StraightLook(this));
			_receptors.Add(new LifeIndicator(this));
			_effectors.Add(new Rotator(this));
			_effectors.Add(new Mover(this));
			X = x;
			Y = y;
			_brain = new Brain(_receptors, _effectors);
		}
	}
}