using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public abstract class NeuralNetwork
	{
		public abstract List<double> Process(List<double> input);

		public List<double> Process(double[] input)
			=> Process(new List<double>(input));
	}
}