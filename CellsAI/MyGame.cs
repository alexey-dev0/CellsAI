using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using ProceduralGenerationLib;
using System.Linq;
using NeuralNetworkLib;
using Microsoft.Xna.Framework.Input;

namespace CellsAI
{
	public class MyGame : WpfGame
	{
		private IGraphicsDeviceService _graphics;
		private SpriteBatch _sprBatch;
		private WpfKeyboard _keyboard;
		private Map _map;

		private int _x;
		private int _y;

		protected override void Initialize()
		{
			Components.Add(new FpsComponent(this));
			_graphics = new WpfGraphicsDeviceService(this);
			_sprBatch = new SpriteBatch(_graphics.GraphicsDevice);
			_keyboard = new WpfKeyboard(this);

			base.Initialize();
		}

		readonly OpenSimplexNoise gen = new OpenSimplexNoise
		{
			Scale = 2,
			Octaves = 5,
			Persistance = 0.3,
			Lacunarity = 3,
			NoiseHeight = 1
		};

		protected override void LoadContent()
		{
			_map = new Map(gen);
			base.LoadContent();
		}

		protected override void Update(GameTime time)
		{
			var keyboardState = _keyboard.GetState();
			if (keyboardState.IsKeyDown(Keys.W)) _y++;
			if (keyboardState.IsKeyDown(Keys.S)) _y--;
			if (keyboardState.IsKeyDown(Keys.A)) _x++;
			if (keyboardState.IsKeyDown(Keys.D)) _x--;
			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			_graphics.GraphicsDevice.Clear(Color.AliceBlue);

			_map.ViewX = _x / 4; // Chunk.CHUNK_SIZE;
			_map.ViewY = _y / 4; /// Chunk.CHUNK_SIZE;
			_map.Draw(_sprBatch);

			base.Draw(time);
		}

		public void Reset()
		{
			if (_map == null) return;
			_map.Dispose();
			_map = new Map(gen);
		}
	}
}
