using NeuralNetworkLib;

namespace CellsAI.Creatures
{
	public abstract class Creature
	{
		private Brain _brain;
		private IReceptor[] _receptors;
		private IEffector[] _effectors;

		// Consider how to create network
		public Creature(NeuralNetwork network)
		{
			// ... receptors & effectors init in child object;
			_brain = new Brain(_receptors, _effectors, network);
		}

		public void Update()
		{
			foreach (var receptor in _receptors)
				receptor.Receive();
			_brain.Update();
			foreach (var effector in _effectors)
				effector.Perform();
		}
	}
}
