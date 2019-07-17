using CellsAI.Entities;
using CellsAI.Entities.Food;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProceduralGenerationLib;
using System;
using System.Collections.Generic;
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
		private readonly List<Drawable> _content;

		public Chunk(OpenSimplexNoise generator, int x, int y)
		{
			_generator = generator;
			_cellGrid = new Cell[CHUNK_SIZE, CHUNK_SIZE];
			_x = x;
			_y = y;
			_content = new List<Drawable>();

			Generate();
		}

		public Chunk(OpenSimplexNoise generator, Vector2 position)
			: this(generator, (int)position.X, (int)position.Y)
		{ }

		private void Generate()
		{
			var noiseHeightMap = SmoothNoise();
			for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					_cellGrid[i, j] = new Cell(this, noiseHeightMap[j, i], _x * CHUNK_SIZE + i, _y * CHUNK_SIZE + j);
		}

		private readonly double _foodRate = 0.05;

		public void GenerateFood()
		{
			var r = new Random(GAME.Seed);
			for (int x = 0; x < CHUNK_SIZE; x++)
				for (int y = 0; y < CHUNK_SIZE; y++)
					if (r.NextDouble() < _foodRate
						&& _cellGrid[x, y].MyType == Cell.CellType.Ground
						&& _cellGrid[x, y].Content.Count == 0)
						new Plant(_x * CHUNK_SIZE + x, _y * CHUNK_SIZE + y);
		}

		private void AddFood()
		{
			var bugCounter = 0;
			var r = new Random();
			int x = r.Next(CHUNK_SIZE);
			int y = r.Next(CHUNK_SIZE);
			while (_cellGrid[x, y].MyType != Cell.CellType.Ground
				|| _cellGrid[x, y].Content.Count != 0)
			{
				x = r.Next(CHUNK_SIZE);
				y = r.Next(CHUNK_SIZE);
				if (bugCounter > 100) return;
				bugCounter++;
			}
			var food = new Plant(_x * CHUNK_SIZE + x, _y * CHUNK_SIZE + y);
			_cellGrid[x, y].Enter(food);
		}

		private double[,] SmoothNoise()
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
			// Island
			for (int x = 0; x < CHUNK_SIZE; x++)
				for (int y = 0; y < CHUNK_SIZE; y++)
				{
					result[y, x] *= GetFading(_x * CHUNK_SIZE + x, _y * CHUNK_SIZE + y);
					if (result[y, x] < 0.6) result[y, x] *= 1.1;
					else if (result[y, x] > 0.6) result[y, x] *= 0.9;
				}
			return result;
		}

		private double GetFading(int x, int y)
		{
			double sqr = x * x + y * y;
			sqr = ISLAND_SIZE - sqr;
			if (sqr < 0) return 0.0;
			sqr /= ISLAND_SIZE;
			//sqr *= sqr;
			return 1.2 * sqr;
		}

		public Texture2D GetTexture(GraphicsDevice graphics)
		{
			if (_texture != null) return _texture;
			_texture = new Texture2D(graphics, CHUNK_SIZE, CHUNK_SIZE);
			Color[] data = new Color[CHUNK_SIZE * CHUNK_SIZE];
			for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					data[i * CHUNK_SIZE + j] = _cellGrid[j, i].Color;
			_texture.SetData(data);
			return _texture;
		}

		private Texture2D _gTex;

		private Texture2D GridTexture
		{
			get
			{
				if (_gTex == null)
				{
					int sz = CELL_SIZE;
					_gTex = new Texture2D(GAME.SprBatch.GraphicsDevice, sz, sz);
					var data = new Color[(int)Math.Pow(sz, 2)];
					for (int i = 0; i < data.Length; i++)
						data[i] = new Color(0, 0, 0, 70);
					_gTex.SetData(data);
				}
				return _gTex;
			}
		}

		public void DrawGrid(Vector2 position, Vector2 subPos)
		{
			GAME.SprBatch.Draw(
				texture: GridTexture,
				position: position,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: new Vector2(SCALE * CHUNK_SIZE),
				effects: SpriteEffects.None,
				layerDepth: 0.9f);
			GAME.SprBatch.Draw(
				texture: GridTexture,
				position: position + subPos * CELL_SIZE * SCALE,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: new Vector2(SCALE),
				effects: SpriteEffects.None,
				layerDepth: 1.0f);
		}

		public void Draw(Vector2 position)
		{
			GAME.SprBatch.Draw(
				texture: GetTexture(GAME.SprBatch.GraphicsDevice),
				position: position,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: new Vector2(CELL_SIZE * SCALE),
				effects: SpriteEffects.None,
				layerDepth: 0f);
			foreach (var entity in _content)
			{
				int eX = (entity.X % CHUNK_SIZE + CHUNK_SIZE) % CHUNK_SIZE;
				int eY = (entity.Y % CHUNK_SIZE + CHUNK_SIZE) % CHUNK_SIZE;
				var entityPos = new Vector2(eX, eY) * CELL_SIZE * SCALE;
				entity.Draw(position + entityPos);
			}
		}

		public Cell this[int x, int y]
		{
			get
			{
				var tx = (x + CHUNK_SIZE) % CHUNK_SIZE;
				var ty = (y + CHUNK_SIZE) % CHUNK_SIZE;
				if (tx != x || ty != y) throw new Exception();

				return _cellGrid[x, y];
			}
		}

		public void Enter(Drawable entity)
		{
			if (!_content.Contains(entity))
				_content.Add(entity);
		}

		public void Leave(Drawable entity)
		{
			if (_content.Contains(entity))
			{
				if (entity is Plant)
					AddFood();
				_content.Remove(entity);
			}
		}

		public void Dispose()
		{
			if (_texture != null)
				_texture.Dispose();
		}
	}
}