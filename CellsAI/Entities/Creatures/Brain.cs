using NeuralNetworkLib;
using System;
using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public class Brain
	{
		private List<IReceptor> _receptors;
		private List<IEffector> _effectors;
		private NeuralNetwork _network;

		public Brain(List<IReceptor> receptors, List<IEffector> effectors, NeuralNetwork network = null)
		{
			_receptors = receptors;
			_effectors = effectors;
			var inputCount = 0;
			foreach (var r in _receptors)
				inputCount += r.Values.Count;
			var outputCount = _effectors.Count;
			if (network == null)
			{
				var newNetwork = new MultilayerPerceptron(inputCount, outputCount, ActivationFunctions.Sigmoid);
				var r = new Random();
				for (int i = 0; i < r.Next(2, 5); i++)
					newNetwork.AddLayer(r.Next(3, 10));
				newNetwork.Connect();
				network = newNetwork;
			}
			_network = network;
		}

		public void Update()
		{
			var inputs = new List<double>();
			for (int i = 0; i < _receptors.Count; i++)
				inputs.AddRange(_receptors[i].Values);
			var outputs = _network.Process(inputs);
			for (int i = 0; i < _effectors.Count; i++)
				_effectors[i].Value = outputs[i];
		}

		public NeuralNetwork GetNetwork()
			=> _network;
	}
}