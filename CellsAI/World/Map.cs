using CellsAI.Game;
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

		public Map()
		{
			_chunks = new Dictionary<Vector2, Chunk>();
			_generator = new OpenSimplexNoise((int)DateTime.Now.Ticks);
		}

		public Map(int seed) : this()
		{
			_generator = new OpenSimplexNoise(seed);
		}

		public Map(OpenSimplexNoise generator) : this()
		{
			_generator = generator;
		}

		private void AddChunk(int x, int y)
			=> AddChunk(new Vector2(x, y));

		private void AddChunk(Vector2 position)
			=> _chunks.Add(position, new Chunk(_generator, position));

		public void Draw(int mx, int my, bool pressed)
		{
			MyGame.SprBatch.Begin(samplerState: SamplerState.PointClamp);
			var width = MyGame.SprBatch.GraphicsDevice.Viewport.Width;
			var height = MyGame.SprBatch.GraphicsDevice.Viewport.Height;

			int chunkHCount = (int)Math.Ceiling(width / ZOOM_FACTOR) + 1;
			int chunkVCount = (int)Math.Ceiling(height / ZOOM_FACTOR) + 1;

			var LUCorner = new Vector2(ViewX - 0.5f * width, ViewY - 0.5f * height);
			var initChunkPoint = new Vector2((float)Math.Floor(LUCorner.X / ZOOM_FACTOR), (float)Math.Floor(LUCorner.Y / ZOOM_FACTOR));
			var initDrawPoint = new Vector2(-LUCorner.X + (int)initChunkPoint.X * ZOOM_FACTOR, -LUCorner.Y + (int)initChunkPoint.Y * ZOOM_FACTOR);

			//Game.DebugInfo.DebugMessage += $"Position: {new Vector2(ViewX, ViewY)}\n";

			for (int x = 0; x < chunkHCount; x++)
				for (int y = 0; y < chunkVCount; y++)
				{
					var chunkPos = new Vector2(initChunkPoint.X + x, initChunkPoint.Y + y);
					var drawPos = initDrawPoint + new Vector2(x * ZOOM_FACTOR, y * ZOOM_FACTOR);
					GetChunk(chunkPos).Draw(drawPos);
				}

			var xx = (LUCorner.X + mx) / (SCALE * CELL_SIZE);
			var yy = (LUCorner.Y + my) / (SCALE * CELL_SIZE);
			int cx = (int)Math.Floor(xx / CHUNK_SIZE);
			int cy = (int)Math.Floor(yy / CHUNK_SIZE);
			int vx = (int)Math.Floor(xx % CHUNK_SIZE);
			int vy = (int)Math.Floor(yy % CHUNK_SIZE);
			vx = (vx + CHUNK_SIZE) % CHUNK_SIZE;
			vy = (vy + CHUNK_SIZE) % CHUNK_SIZE;
			var chunk = GetChunk(cx, cy);
			chunk.DrawGrid(initDrawPoint + new Vector2((cx - initChunkPoint.X) * ZOOM_FACTOR, (cy - initChunkPoint.Y) * ZOOM_FACTOR), new Vector2(vx, vy));
			Game.DebugInfo.DebugMessage += chunk[vx, vy].ToString();
			if (pressed && chunk[vx, vy].Content.Count > 0) chunk[vx, vy].Leave(chunk[vx, vy].Content[0]);

			MyGame.SprBatch.End();
			Game.DebugInfo.DebugMessage += $"CHUNKS: {_chunks.Count}\n";
		}

		private Chunk GetChunk(int x, int y)
			=> GetChunk(new Vector2(x, y));

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
				int chunkX = (int)Math.Floor((double)x / CHUNK_SIZE);
				int chunkY = (int)Math.Floor((double)y / CHUNK_SIZE);
				int cellX = x - chunkX * CHUNK_SIZE;
				int cellY = y - chunkY * CHUNK_SIZE;
				return GetChunk(chunkX, chunkY)[cellX, cellY];
			}
		}

		public void Dispose()
		{
			foreach (var c in _chunks) c.Value.Dispose();
		}
	}
}