using CellsAI.Entities.Creatures.Effectors;
using CellsAI.Entities.Creatures.Receptors;
using NeuralNetworkLib;

namespace CellsAI.Entities.Creatures
{
	internal class SimpleCreature : Creature
	{
		public SimpleCreature(int x, int y, NeuralNetwork network = null, int changeCount = 0) : base()
		{
			_receptors.Add(new StraightLook(this));
			//_receptors.Add(new LifeIndicator(this));
			_effectors.Add(new Rotator(this));
			_effectors.Add(new Mover(this));
			_effectors.Add(new Absorber(this));
			X = x;
			Y = y;
			if (network != null)
			{
				network = network.Copy();
				network.RandomChange(changeCount / 2, 1 + changeCount / 6);
			}
			_brain = new Brain(_receptors, _effectors, network);
		}
	}
}