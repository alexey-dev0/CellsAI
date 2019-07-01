using CellsAI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProceduralGenerationLib;
using System;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.World
{
	public class Map
	{
		public int ViewX;
		public int ViewY;

		private readonly OpenSimplexNoise _generator;
		private readonly Dictionary<Vector2, Chunk> _chunks;
		private readonly List<Entity> _entities;

		public Map(int seed)
		{
			_generator = new OpenSimplexNoise(seed);
			_chunks = new Dictionary<Vector2, Chunk>();
		}

		public Map(OpenSimplexNoise generator)
		{
			_generator = generator;
			_chunks = new Dictionary<Vector2, Chunk>();
		}

		public void Update()
		{
			foreach (var entity in _entities)
				entity.Update();
		}

		public Map() : this((int)DateTime.Now.Ticks) { }

		private void AddChunk(int x, int y)
			=> AddChunk(new Vector2(x, y));

		private void AddChunk(Vector2 position)
			=> _chunks.Add(position, new Chunk(_generator, position));

		public void Draw(SpriteBatch sprBatch)
		{
			sprBatch.Begin(samplerState: SamplerState.PointClamp);
			var width = sprBatch.GraphicsDevice.Viewport.Width;
			var height = sprBatch.GraphicsDevice.Viewport.Height;

			int chunkHCount = (int)Math.Ceiling(width / ZOOM_FACTOR) + 1;
			int chunkVCount = (int)Math.Ceiling(height / ZOOM_FACTOR) + 1;

			var LUCorner = new Vector2(ViewX - 0.5f * width, ViewY - 0.5f * height);
			var initChunkPoint = new Vector2((float)Math.Floor(LUCorner.X / ZOOM_FACTOR), (float)Math.Floor(LUCorner.Y / ZOOM_FACTOR));
			var initDrawPoint = new Vector2(-LUCorner.X + (int)initChunkPoint.X * ZOOM_FACTOR, -LUCorner.Y + (int)initChunkPoint.Y * ZOOM_FACTOR);

			Game.DebugInfo.DebugMessage += $"Position: {new Vector2(ViewX, ViewY)}\n";

			for (int x = 0; x < chunkHCount; x++)
				for (int y = 0; y < chunkVCount; y++)
				{
					var chunkPos = new Vector2(initChunkPoint.X + x, initChunkPoint.Y + y);
					sprBatch.Draw(
						texture: GetChunk(chunkPos).GetTexture(sprBatch.GraphicsDevice),
						position: initDrawPoint + new Vector2(x * ZOOM_FACTOR, y * ZOOM_FACTOR),
						sourceRectangle: null,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: new Vector2(CELL_SIZE * SCALE),
						effects: SpriteEffects.None,
						layerDepth: 0f);
				}

			sprBatch.End();
			Game.DebugInfo.DebugMessage += $"CHUNKS: {_chunks.Count}\n";
		}

		private Chunk GetChunk(int x, int y)
		{
			var position = new Vector2(x, y);
			return GetChunk(position);
		}

		private Chunk GetChunk(Vector2 position)
		{
			position = new Vector2((int)Math.Floor(position.X), (int)Math.Floor(position.Y));
			if (!_chunks.ContainsKey(position)) AddChunk(position);
			return _chunks[position];
		}

		public Cell this[int x, int y]
		{
			get
			{
				int chunkX = x / CHUNK_SIZE;
				int chunkY = y / CHUNK_SIZE;
				return GetChunk(chunkX, chunkY)[x % CHUNK_SIZE, y % CHUNK_SIZE];
			}
		}

		public void AddCreatures()
		{

		}

		public void Dispose()
		{
			foreach (var c in _chunks) c.Value.Dispose();
		}
	}
}