using System;

namespace NeuralNetworkLib
{
	public static class ActivationFunctions
	{
		public static double Identity(double x)
			=> x;

		public static double BinaryStep(double x)
			=> x < 0 ? 0 : 1;

		public static double Sigmoid(double x)
			=> 1.0 / (1.0 + Math.Pow(Math.E, -x));

		public static double TanH(double x)
			=> (Math.Pow(Math.E, x) - Math.Pow(Math.E, -x)) / (Math.Pow(Math.E, x) + Math.Pow(Math.E, -x));

		public static double ArcTan(double x)
			=> Math.Atan(x);

		public static double Gaussian(double x)
			=> Math.Pow(Math.E, -x * x);
	}
}