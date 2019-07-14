using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public abstract class NeuralNetwork
	{
		public string Id;

		protected List<Neuron> _input;
		protected List<Neuron> _output;

		public abstract List<double> Process(List<double> input);

		public List<double> Process(double[] input)
			=> Process(new List<double>(input));

		public abstract NeuralNetwork Copy();

		public abstract void RandomChange(int count, int perNeuron);

		public abstract int NeuronCount();
	}
}