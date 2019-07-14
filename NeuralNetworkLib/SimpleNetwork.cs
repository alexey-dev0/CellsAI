using System;
using System.Collections.Generic;

namespace NeuralNetworkLib
{
	public class SimpleNetwork : NeuralNetwork
	{
		private List<List<double>> _values;
		private List<List<List<double>>> _weights;
		private Func<double, double> _func;
		private Random _rand;

		public SimpleNetwork(int input, int output, Func<double, double> func, params int[] hidden)
		{
			Id = "";
			_func = func;
			_rand = new Random();
			_values = new List<List<double>>();
			_weights = new List<List<List<double>>>();

			_values.Add(GetEmptyList(input));
			foreach (var h in hidden)
				_values.Add(GetEmptyList(h));
			_values.Add(GetEmptyList(output));

			// input empty list (_weights[0])
			_weights.Add(new List<List<double>>());
			for (int i = 1; i < _values.Count; i++)
			{
				_weights.Add(new List<List<double>>());
				for (int j = 0; j < _values[i].Count; j++)
					_weights[i].Add(GetRandomWeights(_values[i - 1].Count));
			}
		}

		private List<double> GetEmptyList(int count)
		{
			var result = new List<double>();
			for (int i = 0; i < count; i++)
				result.Add(0.0);
			return result;
		}

		private List<double> GetRandomWeights(int count)
		{
			var result = new List<double>();
			for (int i = 0; i < count; i++)
				result.Add(_rand.NextDouble() * 2 - 1);
			return result;
		}

		private void Copy(SimpleNetwork other)
		{
			for (int i = 0; i < _weights.Count; i++)
				for (int j = 0; j < _weights[i].Count; j++)
					for (int k = 0; k < _weights[i][j].Count; k++)
						_weights[i][j][k] = other._weights[i][j][k];
			Id = other.Id;
		}

		public override NeuralNetwork Copy()
		{
			var hidden = new List<int>();
			for (int i = 1; i < _values.Count - 1; i++)
				hidden.Add(_values[i].Count);
			var result = new SimpleNetwork(_values[0].Count, _values[_values.Count - 1].Count, _func, hidden.ToArray());
			result.Copy(this);
			return result;
		}

		public override int NeuronCount()
		{
			int result = 0;
			for (int i = 1; i < _values.Count; i++)
				result += _values[i].Count;
			return result;
		}

		public override List<double> Process(List<double> input)
		{
			_values[0] = input;
			for (int i = 1; i < _values.Count; i++)
			{
				for (int j = 0; j < _values[i].Count; j++)
				{
					double sum = 0.0;
					for (int k = 0; k < _values[i - 1].Count; k++)
						sum += _values[i - 1][k] * _weights[i][j][k];
					_values[i][j] = i == _values.Count - 1 ? ActivationFunctions.Sigmoid(sum) : _func(sum);
				}
			}
			return _values[_values.Count - 1];
		}

		private void ResetNeuron(int layer, int neuro)
		{
			for (int weight = 0; weight < _weights[layer][neuro].Count; weight++)
				_weights[layer][neuro][weight] = _rand.NextDouble() * 2 - 1;
		}

		private void AddNeuron(int layer)
		{
			_values[layer].Add(0.0);
			_weights[layer].Add(GetRandomWeights(_values[layer - 1].Count));
			foreach (var n in _weights[layer + 1])
				n.Add(_rand.NextDouble() * 2 - 1);
		}

		public override void RandomChange(int count, int perNeuron)
		{
			while (count-- > 0)
			{
				var layer = _rand.Next(1, _weights.Count);
				if (_rand.NextDouble() > 0.999 && layer != _weights.Count - 1)
				{
					AddNeuron(layer);
					Id += "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"[_rand.Next(62)];
					continue;
				}
				var neuro = _rand.Next(_weights[layer].Count);
				if (_rand.NextDouble() > 0.96)
					ResetNeuron(layer, neuro);
				else
					for (int i = 0; i < perNeuron; i++)
					{
						var weight = _rand.Next(_weights[layer][neuro].Count);
						_weights[layer][neuro][weight] = _rand.NextDouble() * 2 - 1;
					}

				//ID
				//var temp = Id.ToCharArray();
				//temp[_rand.Next(Id.Length)] = (char)_rand.Next(74, 122);
				//Id = new string(temp);
			}
		}
	}
}