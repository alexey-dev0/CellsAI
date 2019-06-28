/* OpenSimplex Noise 2D
 * Refactored code from https://gist.github.com/KdotJPG/b1270127455a94ac5d19
 * Added Map generation
 */

using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace ProceduralGenerationLib
{
	public class OpenSimplexNoise
	{
		private const double STRETCH_2D = -0.211324865405187;    //(1/Math.sqrt(2+1)-1)/2;
		private const double SQUISH_2D = 0.366025403784439;      //(Math.sqrt(2+1)-1)/2;
		private const double NORM_2D = 47.0;

		private byte[] perm;
		private byte[] perm2D;

		private static double[] gradients2D = new double[]
		{
			 5,  2,    2,  5,
			-5,  2,   -2,  5,
			 5, -2,    2, -5,
			-5, -2,   -2, -5,
		};

		private static Contribution2[] lookup2D;

		private class Contribution2
		{
			public double dx, dy;
			public int xsb, ysb;
			public Contribution2 Next;

			public Contribution2(double multiplier, int xsb, int ysb)
			{
				dx = -xsb - multiplier * SQUISH_2D;
				dy = -ysb - multiplier * SQUISH_2D;
				this.xsb = xsb;
				this.ysb = ysb;
			}
		}

		static OpenSimplexNoise()
		{
			var base2D = new int[][]
			{
				new int[] { 1, 1, 0, 1, 0, 1, 0, 0, 0 },
				new int[] { 1, 1, 0, 1, 0, 1, 2, 1, 1 }
			};
			var p2D = new int[] { 0, 0, 1, -1, 0, 0, -1, 1, 0, 2, 1, 1, 1, 2, 2, 0, 1, 2, 0, 2, 1, 0, 0, 0 };
			var lookupPairs2D = new int[] { 0, 1, 1, 0, 4, 1, 17, 0, 20, 2, 21, 2, 22, 5, 23, 5, 26, 4, 39, 3, 42, 4, 43, 3 };

			var contributions2D = new Contribution2[p2D.Length / 4];
			for (int i = 0; i < p2D.Length; i += 4)
			{
				var baseSet = base2D[p2D[i]];
				Contribution2 previous = null, current = null;
				for (int k = 0; k < baseSet.Length; k += 3)
				{
					current = new Contribution2(baseSet[k], baseSet[k + 1], baseSet[k + 2]);
					if (previous == null)
					{
						contributions2D[i / 4] = current;
					}
					else
					{
						previous.Next = current;
					}
					previous = current;
				}
				current.Next = new Contribution2(p2D[i + 1], p2D[i + 2], p2D[i + 3]);
			}

			lookup2D = new Contribution2[64];
			for (var i = 0; i < lookupPairs2D.Length; i += 2)
			{
				lookup2D[lookupPairs2D[i]] = contributions2D[lookupPairs2D[i + 1]];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int FastFloor(double x)
		{
			var xi = (int)x;
			return x < xi ? xi - 1 : xi;
		}

		public OpenSimplexNoise() : this(DateTime.Now.Ticks)
		{
		}

		public OpenSimplexNoise(long seed)
		{
			perm = new byte[256];
			perm2D = new byte[256];
			var source = new byte[256];
			for (int i = 0; i < 256; i++)
			{
				source[i] = (byte)i;
			}
			_seed = seed * 6364136223846793005L + 1442695040888963407L;
			_seed = _seed * 6364136223846793005L + 1442695040888963407L;
			_seed = _seed * 6364136223846793005L + 1442695040888963407L;
			for (int i = 255; i >= 0; i--)
			{
				_seed = _seed * 6364136223846793005L + 1442695040888963407L;
				int r = (int)((_seed + 31) % (i + 1));
				if (r < 0)
				{
					r += (i + 1);
				}
				perm[i] = source[r];
				perm2D[i] = (byte)(perm[i] & 0x0E);
				source[r] = source[i];
			}
		}

		public double Evaluate(double x, double y)
		{
			//Place input coordinates onto grid.
			double stretchOffset = (x + y) * STRETCH_2D;
			double xs = x + stretchOffset;
			double ys = y + stretchOffset;

			//Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
			int xsb = FastFloor(xs);
			int ysb = FastFloor(ys);

			//Skew out to get actual coordinates of rhombus origin. We'll need these later.
			double squishOffset = (xsb + ysb) * SQUISH_2D;
			double xb = xsb + squishOffset;
			double yb = ysb + squishOffset;

			//Compute grid coordinates relative to rhombus origin.
			double xins = xs - xsb;
			double yins = ys - ysb;

			//Sum those together to get a value that determines which region we're in.
			double inSum = xins + yins;

			//Positions relative to origin point.
			double dx0 = x - xb;
			double dy0 = y - yb;

			//We'll be defining these inside the next block and using them afterwards.
			double dx_ext, dy_ext;
			int xsv_ext, ysv_ext;

			double value = 0;

			//Contribution (1,0)
			double dx1 = dx0 - 1 - SQUISH_2D;
			double dy1 = dy0 - 0 - SQUISH_2D;
			double attn1 = 2 - dx1 * dx1 - dy1 * dy1;
			if (attn1 > 0)
			{
				attn1 *= attn1;
				value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, dx1, dy1);
			}

			//Contribution (0,1)
			double dx2 = dx0 - 0 - SQUISH_2D;
			double dy2 = dy0 - 1 - SQUISH_2D;
			double attn2 = 2 - dx2 * dx2 - dy2 * dy2;
			if (attn2 > 0)
			{
				attn2 *= attn2;
				value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, dx2, dy2);
			}

			if (inSum <= 1)
			{ //We're inside the triangle (2-Simplex) at (0,0)
				double zins = 1 - inSum;
				if (zins > xins || zins > yins)
				{ //(0,0) is one of the closest two triangular vertices
					if (xins > yins)
					{
						xsv_ext = xsb + 1;
						ysv_ext = ysb - 1;
						dx_ext = dx0 - 1;
						dy_ext = dy0 + 1;
					}
					else
					{
						xsv_ext = xsb - 1;
						ysv_ext = ysb + 1;
						dx_ext = dx0 + 1;
						dy_ext = dy0 - 1;
					}
				}
				else
				{ //(1,0) and (0,1) are the closest two vertices.
					xsv_ext = xsb + 1;
					ysv_ext = ysb + 1;
					dx_ext = dx0 - 1 - 2 * SQUISH_2D;
					dy_ext = dy0 - 1 - 2 * SQUISH_2D;
				}
			}
			else
			{ //We're inside the triangle (2-Simplex) at (1,1)
				double zins = 2 - inSum;
				if (zins < xins || zins < yins)
				{ //(0,0) is one of the closest two triangular vertices
					if (xins > yins)
					{
						xsv_ext = xsb + 2;
						ysv_ext = ysb + 0;
						dx_ext = dx0 - 2 - 2 * SQUISH_2D;
						dy_ext = dy0 + 0 - 2 * SQUISH_2D;
					}
					else
					{
						xsv_ext = xsb + 0;
						ysv_ext = ysb + 2;
						dx_ext = dx0 + 0 - 2 * SQUISH_2D;
						dy_ext = dy0 - 2 - 2 * SQUISH_2D;
					}
				}
				else
				{ //(1,0) and (0,1) are the closest two vertices.
					dx_ext = dx0;
					dy_ext = dy0;
					xsv_ext = xsb;
					ysv_ext = ysb;
				}
				xsb += 1;
				ysb += 1;
				dx0 = dx0 - 1 - 2 * SQUISH_2D;
				dy0 = dy0 - 1 - 2 * SQUISH_2D;
			}

			//Contribution (0,0) or (1,1)
			double attn0 = 2 - dx0 * dx0 - dy0 * dy0;
			if (attn0 > 0)
			{
				attn0 *= attn0;
				value += attn0 * attn0 * Extrapolate(xsb, ysb, dx0, dy0);
			}

			//Extra Vertex
			double attn_ext = 2 - dx_ext * dx_ext - dy_ext * dy_ext;
			if (attn_ext > 0)
			{
				attn_ext *= attn_ext;
				value += attn_ext * attn_ext * Extrapolate(xsv_ext, ysv_ext, dx_ext, dy_ext);
			}

			return value / NORM_2D;
		}

		private double Extrapolate(int xsb, int ysb, double dx, double dy)
		{
			int index = perm[(perm[xsb & 0xFF] + ysb) & 0xFF] & 0x0E;
			return gradients2D[index] * dx
				+ gradients2D[index + 1] * dy;
		}

		public double Scale { get; set; }
		public int Octaves { get; set; }
		public double Persistance { get; set; }
		public double Lacunarity { get; set; }
		public double NoiseHeight { get; set; }

		private long _seed = 0;

		public double EvaluateNorm(double x, double y, Vector2[] octaveOffsets)
		{
			if (Scale <= 0.0) Scale = 0.0001;

			double amplitude = 1;
			double frequency = 1;
			double noiseHeight = 0;
			for (int i = 0; i < Octaves; i++)
			{
				double sampleX = (x + octaveOffsets[i].X) / Scale * frequency;
				double sampleY = (y + octaveOffsets[i].Y) / Scale * frequency;

				var noiseValue = Evaluate(sampleX, sampleY);
				noiseHeight += noiseValue * amplitude;

				amplitude *= Persistance;
				frequency *= Lacunarity;
			}

			return noiseHeight;
		}

		private double InverseLerp(double min, double max, double value)
			=> Math.Abs((value - min) / (max - min));

		public double[,] GetValueMap(int width, int height, Vector2 offset)
		{
			double maxPossibleHeight = 0;
			var r = new Random((int)_seed);
			var octaveOffsets = new Vector2[Octaves];
			for (int i = 0; i < Octaves; i++)
			{
				float offsetX = r.Next(-100000, 100000) + offset.X;
				float offsetY = r.Next(-100000, 100000) + offset.Y;
				octaveOffsets[i] = new Vector2(offsetX, offsetY);
				maxPossibleHeight += Math.Pow(Persistance, i);
			}
			maxPossibleHeight *= 0.6;

			var result = new double[height, width];
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
					result[i, j] = EvaluateNorm(j, i, octaveOffsets);

			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
					result[i, j] = Math.Min(1.0, Math.Max(0.0, InverseLerp(-maxPossibleHeight, maxPossibleHeight, result[i, j])));

			return result;
		}

		public OpenSimplexNoise(OpenSimplexNoise other) : this(other._seed)
		{
			Scale = other.Scale;
			Octaves = other.Octaves;
			Persistance = other.Persistance;
			Lacunarity = other.Lacunarity;
			NoiseHeight = other.NoiseHeight;
		}
	}
}