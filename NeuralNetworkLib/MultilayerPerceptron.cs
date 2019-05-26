using System;
using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public class MultilayerPerceptron
	{
		private List<Neuron> _input;
		private List<List<Neuron>> _hidden;
		private List<Neuron> _output;
		private Func<double, double> _func;

		private MultilayerPerceptron()
		{
			_input = new List<Neuron>();
			_hidden = new List<List<Neuron>>();
			_output = new List<Neuron>();
		}

		public MultilayerPerceptron(int inputs, int outputs, Func<double, double> func) : this()
		{
			while (inputs-- > 0)
				_input.Add(new Neuron(Neuron.NeuronType.Input));
			while (outputs-- > 0)
				_output.Add(new Neuron(Neuron.NeuronType.Output, func));
			_func = func;
		}

		public void AddLayer(int count)
		{
			var l = new List<Neuron>();
			while (count-- > 0)
				l.Add(new Neuron(Neuron.NeuronType.Hidden, _func));
			_hidden.Add(l);
		}

		public void Connect() // Only straight at this moment
		{
			if (_hidden.Count == 0)
				foreach (var n in _output)
					n.AddParents(_input);
			else
			{
				foreach (var n in _hidden[0])
					n.AddParents(_input);
				for (int i = 1; i < _hidden.Count; i++)
					foreach (var n in _hidden[i])
						n.AddParents(_hidden[i - 1]);
				foreach (var n in _output)
					n.AddParents(_hidden[_hidden.Count - 1]);
				foreach (var n in _hidden[_hidden.Count - 1])
					n.AddChildren(_output);
				for (int i = 0; i < _hidden.Count - 1; i++)
					foreach (var n in _hidden[i])
						n.AddChildren(_hidden[i + 1]);
			}
		}

		public List<double> Process(List<double> input)
		{
			var result = new List<double>();
			for (int i = 0; i < input.Count; i++)
				_input[i].Result = input[i];
			foreach (var n in _output)
				result.Add(n.Result);
			return result;
		}
	}
}