using NeuralNetworkLib;

namespace CellsAI.Creatures
{
	public class Brain
	{
		private IReceptor[] _receptors;
		private IEffector[] _effectors;
		private NeuralNetwork _network;

		public Brain(IReceptor[] receptors, IEffector[] effectors, NeuralNetwork network)
		{
			_receptors = receptors;
			_effectors = effectors;
			_network = network;
		}

		public void Update()
		{
			var inputs = new double[_receptors.Length];
			for (int i = 0; i < _receptors.Length; i++)
				inputs[i] = _receptors[i].Value;
			var outputs = _network.Process(inputs);
			for (int i = 0; i < _effectors.Length; i++)
				_effectors[i].Value = outputs[i];
		}
	}
}
