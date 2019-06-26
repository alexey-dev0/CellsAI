using NeuralNetworkLib;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public class Brain
	{
		private List<IReceptor> _receptors;
		private List<IEffector> _effectors;
		private NeuralNetwork _network;

		public Brain(List<IReceptor> receptors, List<IEffector> effectors, NeuralNetwork network)
		{
			_receptors = receptors;
			_effectors = effectors;
			_network = network;
		}

		public void Update()
		{
			var inputs = new double[_receptors.Count];
			for (int i = 0; i < _receptors.Count; i++)
				inputs[i] = _receptors[i].Value;
			var outputs = _network.Process(inputs);
			for (int i = 0; i < _effectors.Count; i++)
				_effectors[i].Value = outputs[i];
		}
	}
}