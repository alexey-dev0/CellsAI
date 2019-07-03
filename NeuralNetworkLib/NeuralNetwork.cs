using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public abstract class NeuralNetwork
	{
		protected List<Neuron> _input;
		protected List<Neuron> _output;

		public NeuralNetwork(int inputCount, int outputCount)
		{
			_input = new List<Neuron>();
			_output = new List<Neuron>();
			while (inputCount-- > 0)
				_input.Add(new Neuron(Neuron.NeuronType.Input));
			while (outputCount-- > 0)
				_output.Add(new Neuron(Neuron.NeuronType.Output, 
					ActivationFunctions.Sigmoid)); ;
		}

		public abstract List<double> Process(List<double> input);

		public List<double> Process(double[] input)
			=> Process(new List<double>(input));
	}
}