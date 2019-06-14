using Microsoft.Xna.Framework;

namespace ProceduralGenerationLib
{
	public struct Cell
	{
		public static readonly int SIZE = 4;

		// [0; 1]
		private double _height;
		private double _temperature;

		public Cell(double height, double temperature)
		{
			_height = height;
			_temperature = temperature;
		}

		public Color Color => ColorFromHeight();

		private Color ColorFromHeight()
		{
			_height *= 12;
			_height = (int)_height;
			_height /= 12;
			Color col;

			var water = new double[] { 0.0, 0.3 };
			var sand = new double[] { 0.3, 0.4 };
			var ground = new double[] { 0.4, 0.8 };
			var stone = new double[] { 0.8, 1.0 };

			double[] segment;

			if (_height < water[1]) segment = water;
			else if (_height < sand[1]) segment = sand;
			else if (_height < ground[1]) segment = ground;
			else segment = stone;

			_height = 1.0 / (segment[1] - segment[0]) * (_height - segment[0]);

			if (segment == water) col = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, (float)_height);
			else if (segment == sand) col = Color.Lerp(Color.Wheat, Color.Tan, (float)_height);
			else if (segment == ground) col = Color.Lerp(Color.YellowGreen, Color.DarkGreen, (float)_height);
			else col = Color.Lerp(new Color(0x54, 0x3D, 0x21), Color.Snow, (float)_height);

			var tempCol = Color.Lerp(Color.DodgerBlue, Color.OrangeRed, (float)_temperature);
			byte R = (byte)(col.R * 0.5f + tempCol.R * 0.5f);
			byte G = (byte)(col.G * 0.5f + tempCol.G * 0.5f);
			byte B = (byte)(col.B * 0.5f + tempCol.B * 0.5f);

			return new Color(R, G, B);
		}
	}
}