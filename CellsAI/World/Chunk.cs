using CellsAI.Entities;
using CellsAI.Entities.Creatures;
using CellsAI.Game;
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
		private List<Drawable> _content;

		public Chunk(OpenSimplexNoise generator, int x, int y)
		{
			_generator = generator;
			_cellGrid = new Cell[CHUNK_SIZE, CHUNK_SIZE];
			_x = x;
			_y = y;
			_content = new List<Drawable>();

			Generate();
			GenerateFood();
		}

		public Chunk(OpenSimplexNoise generator, Vector2 position)
			: this(generator, (int)position.X, (int)position.Y)
		{ }

		private void Generate()
		{
			var noiseHeightMap = smoothNoise();
			for (int i = 0; i < CHUNK_SIZE; i++)
				for (int j = 0; j < CHUNK_SIZE; j++)
					_cellGrid[i, j] = new Cell(this, noiseHeightMap[j, i]);
		}

		private double _foodRate = 0.02;

		private void GenerateFood()
		{
			var r = new Random(MyGame.Seed);
			for (int x = 0; x < CHUNK_SIZE; x++)
				for (int y = 0; y < CHUNK_SIZE; y++)
					if (r.NextDouble() < _foodRate
						&& _cellGrid[x, y].MyType == Cell.CellType.Ground
						&& _cellGrid[x, y].Content.Count == 0)
					{
						var food = new Food(_x * CHUNK_SIZE + x, _y * CHUNK_SIZE + y);
						_cellGrid[x, y].Content.Add(food);
						_content.Add(food);
					}
						
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
					int sz = CHUNK_SIZE * CELL_SIZE;
					_gTex = new Texture2D(MyGame.SprBatch.GraphicsDevice, sz, sz);
					var data = new Color[(int)Math.Pow(sz, 2)];
					for (int i = 0; i < data.Length; i++)
						if (i / sz == 0 || i % sz == 0
							|| i / sz == sz - 1 || i % sz == sz - 1)
							data[i] = Color.Black;
						else
							data[i] = Color.Transparent;
					_gTex.SetData(data);
				}
				return _gTex;
			}
		}

		public void Draw(Vector2 position)
		{
			MyGame.SprBatch.Draw(
				texture: GetTexture(MyGame.SprBatch.GraphicsDevice),
				position: position,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: new Vector2(CELL_SIZE * SCALE),
				effects: SpriteEffects.None,
				layerDepth: 0f);
			//MyGame.SprBatch.Draw(
			//	texture: GridTexture,
			//	position: position,
			//	sourceRectangle: null,
			//	color: Color.White,
			//	rotation: 0f,
			//	origin: Vector2.Zero,
			//	scale: new Vector2(SCALE),
			//	effects: SpriteEffects.None,
			//	layerDepth: 0f);
			//MyGame.SprBatch.DrawString(DebugInfo.DefaultFont, $"{_x}, {_y}", position, Color.White);
			foreach (var entity in _content)
			{
				int eX = (entity.X % CHUNK_SIZE + CHUNK_SIZE) % CHUNK_SIZE;
				int eY = (entity.Y % CHUNK_SIZE + CHUNK_SIZE) % CHUNK_SIZE;
				var entityPos = new Vector2(eX, eY);
				entityPos *= CELL_SIZE * SCALE;
				float rot = entity is Creature ? (float)(Math.PI / 4 * (int)(entity as Creature).MyRotation) : 0f;
				MyGame.SprBatch.Draw(
					texture: entity.GetTexture(),
					position: position + entityPos,
					sourceRectangle: null,
					color: Color.White,
					rotation: rot,
					origin: new Vector2(CELL_SIZE / 2),
					scale: new Vector2(SCALE),
					effects: SpriteEffects.None,
					layerDepth: 0f);
				//MyGame.SprBatch.DrawString(
				//	DebugInfo.DefaultFont,
				//	$"{eX}, {eY}",
				//	position + entityPos - new Vector2(-5),
				//	Color.White);
			}
			//if (_x == 0 && _y == 0) throw new Exception();
		}

		public void Update() // Seems no reason
		{
			foreach (var entity in _content)
			{
				entity.Update();
			}
		}

		public Cell this[int x, int y]
		{
			get
			{
				x = (x + CHUNK_SIZE) % CHUNK_SIZE;
				y = (y + CHUNK_SIZE) % CHUNK_SIZE;
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
				_content.Remove(entity);
		}

		public void Dispose()
		{
			if (_texture != null)
				_texture.Dispose();
		}
	}
}