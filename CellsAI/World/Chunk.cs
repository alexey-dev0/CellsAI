using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProceduralGenerationLib;
using System;
using static CellsAI.Game.GameParameters;

namespace CellsAI.World
{
	public class Chunk
	{
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

		public Chunk(OpenSimplexNoise generator, Vector2 position)
			: this(generator, (int)position.X, (int)position.Y)
		{ }

		private void Generate()
		{
			var noiseHeightMap = smoothNoise();for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					_cellGrid[i, j] = new Cell(this, noiseHeightMap[i, j]);
		}

		private double[,] smoothNoise()
		{
			var result = new double[CHUNK_SIZE, CHUNK_SIZE];
			var radius = 1;
			for (int i = -radius; i <= radius; i++)
				for (int j = -radius; j <= radius; j++)
				{
					var map = _generator.GetValueMap(CHUNK_SIZE, CHUNK_SIZE,
						new Vector2(_x * CHUNK_SIZE + i, _y * CHUNK_SIZE + j));
					for (int x = 0; x < CHUNK_SIZE; x++)
						for (int y = 0; y < CHUNK_SIZE; y++)
							result[x, y] += map[x, y];
				}
			for (int x = 0; x < CHUNK_SIZE; x++)
				for (int y = 0; y < CHUNK_SIZE; y++)
					result[x, y] /= Math.Pow(radius * 2 + 1, 2);
			return result;
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

		public void Draw(SpriteBatch sprBatch)
		{
			sprBatch.Begin();

			sprBatch.End();
		}

		public Cell this[int x, int y]
		{
			get { return _cellGrid[x, y]; }
		}

		public void Dispose()
		{
			if (_texture != null)
				_texture.Dispose();
		}
	}
}