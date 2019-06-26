using CellsAI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProceduralGenerationLib;
using System;
using System.Collections.Generic;

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

		public void Draw(SpriteBatch sprBatch)
		{
			sprBatch.Begin(samplerState: SamplerState.PointClamp);
			var width = sprBatch.GraphicsDevice.Viewport.Width;
			var height = sprBatch.GraphicsDevice.Viewport.Height;

			var chunkSZ = Chunk.CHUNK_SIZE * GameConstants.CELL_SIZE;
			var centerX = (int)(0.5 * (width - chunkSZ));
			var centerY = (int)(0.5 * (height - chunkSZ));
			var radius = 4;

			for (int i = -radius; i <= radius; i++)
				for (int j = -radius; j <= radius; j++)
					sprBatch.Draw(
						texture: this[ViewX + i, ViewY + j].GetTexture(sprBatch.GraphicsDevice),
						position: new Vector2(centerX - i * chunkSZ, centerY - j * chunkSZ),
						sourceRectangle: null,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: GameConstants.CELL_SIZE,
						effects: SpriteEffects.None,
						layerDepth: 0f);
			sprBatch.End();
		}

		public Chunk this[int x, int y]
		{
			get
			{
				var position = new Vector2(x, y);
				if (!_chunks.ContainsKey(position)) AddChunk(x, y);
				return _chunks[position];
			}
		}

		public void Dispose()
		{
			foreach (var c in _chunks) c.Value.Dispose();
		}
	}
}