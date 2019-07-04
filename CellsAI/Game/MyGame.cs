using CellsAI.Views;
using CellsAI.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using ProceduralGenerationLib;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Game
{
	public class MyGame : WpfGame
	{
		private IGraphicsDeviceService _graphics;
		public static SpriteBatch SprBatch;
		private WpfKeyboard _keyboard;
		private WpfMouse _mouse;

		private static Map _world;
		private CreatureController _controller;
		public MainWindow Win;

		public static Map World
		{
			get
			{
				if (_world == null) _world = new Map(_generator);
				return _world;
			}

			set { _world = value; }
		}

		public static int Seed { get; set; }

		private int _x;
		private int _y;

		protected override void Initialize()
		{
			Components.Add(new DebugInfo(this));
			_graphics = new WpfGraphicsDeviceService(this);
			SprBatch = new SpriteBatch(_graphics.GraphicsDevice);
			_keyboard = new WpfKeyboard(this);
			_mouse = new WpfMouse(this);
			_controller = new CreatureController();

			Win.AddSlider("Scale", 1, 100, 20);
			Win.AddSlider("Octaves", 1, 8, 4);
			Win.AddSlider("Persistance", 0.1, 2.0, 0.3);
			Win.AddSlider("Lacunarity", 1, 10, 3);
			Win.AddSlider("NoiseHeight", 0, 2, 1);

			Reset();

			base.Initialize();
		}

		private static OpenSimplexNoise _generator = new OpenSimplexNoise
		{
			Scale = 40,
			Octaves = 5,
			Persistance = 0.3,
			Lacunarity = 3,
			NoiseHeight = 1
		};

		protected override void LoadContent()
		{
			_controller.AddCreatures(100);
			base.LoadContent();
		}

		protected override void Update(GameTime time)
		{
			var keyboardState = _keyboard.GetState();
			int velocity;
			velocity = keyboardState.IsKeyDown(Keys.LeftShift) ? 10 : 1;
			if (keyboardState.IsKeyDown(Keys.W)) _y -= velocity;
			if (keyboardState.IsKeyDown(Keys.S)) _y += velocity;
			if (keyboardState.IsKeyDown(Keys.A)) _x -= velocity;
			if (keyboardState.IsKeyDown(Keys.D)) _x += velocity;
			SCALE = 1 + _mouse.GetState().ScrollWheelValue / 1000.0f;

			_controller.Update();

			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			_graphics.GraphicsDevice.Clear(Color.Purple);

			World.ViewX = _x;
			World.ViewY = _y;
			World.Draw();

			base.Draw(time);
		}

		public void Reset()
		{
			if (World == null) return;

			_generator = new OpenSimplexNoise(13)
			{
				Scale = MainWindow.Sliders["Scale"].Value,
				Octaves = (int)MainWindow.Sliders["Octaves"].Value,
				Persistance = MainWindow.Sliders["Persistance"].Value,
				Lacunarity = MainWindow.Sliders["Lacunarity"].Value,
				NoiseHeight = MainWindow.Sliders["NoiseHeight"].Value
			};

			World.Dispose();
			World = new Map(_generator);
		}
	}
}