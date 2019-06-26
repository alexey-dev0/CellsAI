using CellsAI.Entities.Creatures;
using CellsAI.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using ProceduralGenerationLib;
using System.Collections.Generic;

namespace CellsAI.Game
{
	public class MyGame : WpfGame
	{
		private IGraphicsDeviceService _graphics;
		private SpriteBatch _sprBatch;
		private WpfKeyboard _keyboard;
		private List<Creature> _creatures;

		private static Map _world;

		public static Map World
		{
			get
			{
				if (_world == null) _world = new Map(_generator);
				return _world;
			}

			set
			{
				_world = value;
			}
		}

		public static int Seed { get; set; }

		private int _x;
		private int _y;

		protected override void Initialize()
		{
			Components.Add(new FpsComponent(this));
			_graphics = new WpfGraphicsDeviceService(this);
			_sprBatch = new SpriteBatch(_graphics.GraphicsDevice);
			_keyboard = new WpfKeyboard(this);
			_creatures = new List<Creature>();

			base.Initialize();
		}

		private static readonly OpenSimplexNoise _generator = new OpenSimplexNoise
		{
			Scale = 2,
			Octaves = 5,
			Persistance = 0.3,
			Lacunarity = 3,
			NoiseHeight = 1
		};

		protected override void LoadContent()
		{
			for (int i = 0; i < 10; i++)
				_creatures.Add(new SimpleCreature(_sprBatch, i * GameConstants.CELL_SIZE, i * GameConstants.CELL_SIZE));
			base.LoadContent();
		}

		protected override void Update(GameTime time)
		{
			var keyboardState = _keyboard.GetState();
			if (keyboardState.IsKeyDown(Keys.W)) _y++;
			if (keyboardState.IsKeyDown(Keys.S)) _y--;
			if (keyboardState.IsKeyDown(Keys.A)) _x++;
			if (keyboardState.IsKeyDown(Keys.D)) _x--;

			foreach (var creature in _creatures)
				creature.Update();

			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			_graphics.GraphicsDevice.Clear(Color.AliceBlue);

			World.ViewX = _x / 4; // Chunk.CHUNK_SIZE;
			World.ViewY = _y / 4; // Chunk.CHUNK_SIZE;
			World.Draw(_sprBatch);

			foreach (var creature in _creatures)
				creature.Draw(_sprBatch, creature.X * GameConstants.CELL_SIZE, creature.Y * GameConstants.CELL_SIZE);

			base.Draw(time);
		}

		public void Reset()
		{
			if (World == null) return;
			World.Dispose();
			World = new Map(_generator);
		}
	}
}