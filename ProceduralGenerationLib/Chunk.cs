using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProceduralGenerationLib
{
	public class Chunk
	{
		public static readonly int CHUNK_SIZE = 16;
		public static int K = 1;

		private readonly OpenSimplexNoise _generator;
		private readonly int _x;
		private readonly int _y;
		private readonly Cell[,] _cellGrid;
		private Texture2D _texture;

		public Chunk(OpenSimplexNoise generator, int x, int y)
		{
			_generator = generator;
			_cellGrid = new Cell[CHUNK_SIZE, CHUNK_SIZE];
			_x = x;
			_y = y;

			Generate();
		}

		private void Generate()
		{
			var noiseHeightMap = _generator.GetValueMap(CHUNK_SIZE, CHUNK_SIZE, new Vector2(_x * CHUNK_SIZE, _y * CHUNK_SIZE));
			var gen2 = new OpenSimplexNoise(_generator);
			gen2.Scale *= 20;
			var noiseTemperatureMap = gen2.GetValueMap(CHUNK_SIZE, CHUNK_SIZE, new Vector2(_x * CHUNK_SIZE, _y * CHUNK_SIZE));
			for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					_cellGrid[i, j] = new Cell(noiseHeightMap[i, j], noiseTemperatureMap[i, j]);
		}

		public Texture2D GetTexture(GraphicsDevice graphics)
		{
			if (_texture != null) return _texture;
			_texture = new Texture2D(graphics, CHUNK_SIZE, CHUNK_SIZE);
			Color[] data = new Color[CHUNK_SIZE * CHUNK_SIZE];
			for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					data[i * CHUNK_SIZE + j] = _cellGrid[i, j].Color;
			_texture.SetData(data);
			return _texture;
		}

		public void Dispose()
		{
			if (_texture != null) _texture.Dispose();
		}
	}
}