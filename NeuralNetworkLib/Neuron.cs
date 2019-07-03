using System;
using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public class Neuron
	{
		public enum NeuronType
		{
			Input,
			Hidden,
			Output
		}

		private double _result;

		public double Result
		{
			get
			{
				if (_type == NeuronType.Input)
					return _result;
				else
				{
					_result = 0;
					for (int i = _weights.Count - 1; i >= 0; i--)
					{
						if (_weights[i] == 0.0)
						{
							_weights.RemoveAt(i);
							_parents.RemoveAt(i);
						}
						else
							_result += _parents[i].Result * _weights[i];
					}
					_result = _activationFunc(_result);
					return _result;
				}
			}
			set
			{
				if (_type == NeuronType.Input)
					_result = value;
			}
		}

		private List<Neuron> _parents;
		private List<double> _weights;
		private List<Neuron> _children;
		private NeuronType _type;
		private Func<double, double> _activationFunc;
		private Random _rand;

		public Neuron(NeuronType type = NeuronType.Hidden)
		{
			if (type != NeuronType.Input)
			{
				_parents = new List<Neuron>();
				_weights = new List<double>();
			}
			if (type != NeuronType.Output) _children = new List<Neuron>();
			_type = type;
			_rand = new Random();
		}

		public Neuron(NeuronType type, double result) : this(type)
		{
			if (type == NeuronType.Input) _result = result;
		}

		public Neuron(NeuronType type, Func<double, double> activationFunc) : this(type) 
			=> _activationFunc = activationFunc;

		public void AddParent(Neuron neuron, double weight = -1)
		{
			_parents.Add(neuron);
			if (weight == -1) _weights.Add(_rand.NextDouble());
			else _weights.Add(weight);
		}

		public void AddParents(List<Neuron> neurons, List<double> weights = null)
		{
			_parents.AddRange(neurons);
			if (weights == null)
			{
				for (int i = 0; i < neurons.Count; i++)
					_weights.Add(_rand.NextDouble());
			}
			else
				_weights.AddRange(weights);
			ResetWeights();
		}

		public void AddChild(Neuron neuron) => _children.Add(neuron);

		public void AddChildren(List<Neuron> neurons) => _children.AddRange(neurons);

		public void ResetWeights()
		{
			for (int i = 0; i < _weights.Count; i++)
				_weights[i] = _rand.NextDouble();
		}

		public void RandomChange(int count = 1)
		{
			while (count-- > 0)
				_weights[_rand.Next(_weights.Count)] = _rand.NextDouble();
		}
	}
}