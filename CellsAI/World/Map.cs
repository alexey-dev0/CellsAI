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

		public Map() : this((int)DateTime.Now.Ticks)
		{
		}

		private void AddChunk(int x, int y)
			=> _chunks.Add(new Vector2(x, y), new Chunk(_generator, x, y));

		public void DrawTODO(SpriteBatch sprBatch)
		{
			sprBatch.Begin(samplerState: SamplerState.PointClamp);
			var width = sprBatch.GraphicsDevice.Viewport.Width;
			var height = sprBatch.GraphicsDevice.Viewport.Height;

			var chunkSZ = CHUNK_SIZE * CELL_SIZE;
			var centerX = (int)(0.5 * (width - chunkSZ));
			var centerY = (int)(0.5 * (height - chunkSZ));
			var radius = 4;

			for (int i = -radius; i <= radius; i++)
				for (int j = -radius; j <= radius; j++)
					sprBatch.Draw(
						texture: GetChunk(ViewX + i, ViewY + j).GetTexture(sprBatch.GraphicsDevice),
						position: new Vector2(centerX - i * chunkSZ, centerY - j * chunkSZ),
						sourceRectangle: null,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: CELL_SIZE,
						effects: SpriteEffects.None,
						layerDepth: 0f);
			sprBatch.End();
		}

		public void Draw(SpriteBatch sprBatch)
		{
			sprBatch.Begin(samplerState: SamplerState.PointClamp);
			var width = sprBatch.GraphicsDevice.Viewport.Width;
			var height = sprBatch.GraphicsDevice.Viewport.Height;

			var chunkHCount = Math.Ceiling(width * ZOOM_FACTOR) + 1;
			var chunkVCount = Math.Ceiling(height * ZOOM_FACTOR) + 1;

			var LUCorner = new Vector2(ViewX - 0.5f * width, ViewY - 0.5f * height);
			var initChunkPoint = new Vector2(LUCorner.X / CHUNK_FULL_SIZE, LUCorner.Y / CHUNK_FULL_SIZE);
			var initDrawPoint = new Vector2(LUCorner.X - (int)initChunkPoint.X * CHUNK_FULL_SIZE, LUCorner.Y - (int)initChunkPoint.Y * CHUNK_FULL_SIZE);

			Game.DebugInfo.DebugMessage += $"Position: {new Vector2(ViewX, ViewY)}\n";
			Game.DebugInfo.DebugMessage += $"LUCorner: {LUCorner}\n";
			Game.DebugInfo.DebugMessage += $"DrawPoint: {initDrawPoint}\n";

			for (int x = 0; x < chunkHCount; x++)
				for (int y = 0; y < chunkVCount; y++)
				{
					var chunkPos = new Vector2(initChunkPoint.X - x, initChunkPoint.Y - y);
					sprBatch.Draw(
						texture: GetChunk((int)chunkPos.X, (int)chunkPos.Y).GetTexture(sprBatch.GraphicsDevice),
						position: initDrawPoint + new Vector2(x * CHUNK_FULL_SIZE * SCALE, y * CHUNK_FULL_SIZE * SCALE),
						sourceRectangle: null,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: new Vector2(CELL_SIZE * SCALE),
						effects: SpriteEffects.None,
						layerDepth: 0f);
					//sprBatch.DrawString(
					//	Game.DebugInfo.DefaultFont,
					//	$"{(int)chunkPos.X}, {(int)chunkPos.Y}",
					//	initDrawPoint + new Vector2(x * CHUNK_FULL_SIZE * SCALE, y * CHUNK_FULL_SIZE * SCALE),
					//	Color.Black);
				}

			sprBatch.End();
			Game.DebugInfo.DebugMessage += $"CHUNKS: {_chunks.Count}\n";
		}

		private Chunk GetChunk(int x, int y)
		{
			var position = new Vector2(x, y);
			if (!_chunks.ContainsKey(position)) AddChunk(x, y);
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

		public void Dispose()
		{
			foreach (var c in _chunks) c.Value.Dispose();
		}
	}
}