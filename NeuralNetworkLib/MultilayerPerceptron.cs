using System;
using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public class MultilayerPerceptron : NeuralNetwork
	{
		private readonly List<List<Neuron>> _hidden;

		// Activation functions
		private readonly Func<double, double> _func;

		public MultilayerPerceptron(int inputCount, int outputCount, Func<double, double> func)
		{
			_input = new List<Neuron>();
			_output = new List<Neuron>();
			while (inputCount-- > 0)
				_input.Add(new Neuron(Neuron.NeuronType.Input));
			while (outputCount-- > 0)
				_output.Add(new Neuron(Neuron.NeuronType.Output, func));
			_hidden = new List<List<Neuron>>();
			_func = func;
		}

		// Adding hidden layer with "count" neurons in it
		public void AddLayer(int count)
		{
			var l = new List<Neuron>();
			while (count-- > 0)
				l.Add(new Neuron(Neuron.NeuronType.Hidden, _func));
			_hidden.Add(l);
		}

		// Set connections between neurons
		public void Connect()
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

		private void Copy(MultilayerPerceptron other)
		{
			foreach (var l in other._hidden)
				AddLayer(l.Count);

			if (_hidden.Count == 0)
				for (int i = 0; i < _output.Count; i++)
					_output[i].AddParents(_input, other._output[i]);
			else
			{
				for (int i = 0; i < _hidden[0].Count; i++)
					_hidden[0][i].AddParents(_input, other._hidden[0][i]);
				for (int i = 1; i < _hidden.Count; i++)
					for (int j = 0; j < _hidden[i].Count; j++)
						_hidden[i][j].AddParents(_hidden[i - 1], other._hidden[i][j]);
				for (int i = 0; i < _output.Count; i++)
					_output[i].AddParents(_hidden[_hidden.Count - 1], other._output[i]);
			}
		}

		public override NeuralNetwork Copy()
		{
			var result = new MultilayerPerceptron(_input.Count, _output.Count, _func);
			result.Copy(this);
			return result;
		}

		// Process the input values list & get result values list
		public override List<double> Process(List<double> input)
		{
			var result = new List<double>();
			for (int i = 0; i < input.Count; i++)
				_input[i].Result = input[i];
			foreach (var n in _output)
				result.Add(n.Result);
			return result;
		}

		private void AddNeuron(int i)
		{
			var neuron = new Neuron(Neuron.NeuronType.Hidden, _func);
			if (i == 0) neuron.AddParents(_input);
			else neuron.AddParents(_hidden[i - 1]);
			if (i == _hidden.Count - 1)
				foreach (var n in _output)
					n.AddParent(neuron);
			else
				foreach (var n in _hidden[i + 1])
					n.AddParent(neuron);
			_hidden[i].Add(neuron);
		}

		public override void RandomChange(int count = 1, int perNeuron = 1)
		{
			var rand = new Random();
			while (count-- > 0)
			{
				if (rand.NextDouble() > 0.9999)
					AddNeuron(rand.Next(_hidden.Count));

				Neuron n;
				if (rand.NextDouble() > 0.7)
					n = _output[rand.Next(_output.Count)];
				else
				{
					int i = rand.Next(_hidden.Count);
					n = _hidden[i][rand.Next(_hidden[i].Count)];
				}
				if (rand.NextDouble() > 0.9)
					n.ResetWeights();
				else
					n.RandomChange(perNeuron);
			}
		}

		public override int NeuronCount()
		{
			int result = 0;
			foreach (var l in _hidden)
				result += l.Count;
			return result + _output.Count;
		}
	}
}