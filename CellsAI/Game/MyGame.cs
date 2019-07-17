using CellsAI.Views;
using CellsAI.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using ProceduralGenerationLib;
using System.Collections.Generic;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Game
{
	public class MyGame : WpfGame
	{
		private IGraphicsDeviceService _graphics;
		public SpriteBatch SprBatch;
		private WpfKeyboard _keyboard;
		private WpfMouse _mouse;

		public List<Spawner> _spawners;
		public MainWindow Win;

		public Map World { get; set; }

		public int Seed { get; set; }

		private int _x;
		private int _y;

		public MyGame()
		{
			GAME = this;
		}

		protected override void Initialize()
		{
			Components.Add(new DebugInfo(this));
			_graphics = new WpfGraphicsDeviceService(this);
			SprBatch = new SpriteBatch(_graphics.GraphicsDevice);
			_keyboard = new WpfKeyboard(this);
			_mouse = new WpfMouse(this);
			World = new Map(_generator);

			_spawners = new List<Spawner>();

			//Win.AddSlider("Scale", 1, 100, 70);
			//Win.AddSlider("Octaves", 1, 8, 4);
			//Win.AddSlider("Persistance", 0.1, 2.0, 0.3);
			//Win.AddSlider("Lacunarity", 1, 10, 3);
			//Win.AddSlider("NoiseHeight", 0, 2, 1);

			Win.AddSlider("UpdatePerFrame", -100, 1000, 1);

			//Reset();
			base.Initialize();
		}

		private static readonly OpenSimplexNoise _generator = new OpenSimplexNoise
		{
			Scale = 70,
			Octaves = 4,
			Persistance = 0.3,
			Lacunarity = 3,
			NoiseHeight = 1
		};

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		private int _counter;
		private int _prevScrlVal = 0;

		protected override void Update(GameTime time)
		{
			var keyboardState = _keyboard.GetState();
			int velocity;
			velocity = keyboardState.IsKeyDown(Keys.LeftShift) ? 10 : 1;
			if (keyboardState.IsKeyDown(Keys.W)) _y -= velocity;
			if (keyboardState.IsKeyDown(Keys.S)) _y += velocity;
			if (keyboardState.IsKeyDown(Keys.A)) _x -= velocity;
			if (keyboardState.IsKeyDown(Keys.D)) _x += velocity;
			//SCALE = 1 +  / 10000.0f;
			var diff = _mouse.GetState().ScrollWheelValue - _prevScrlVal;
			if (diff > 0) SCALE += SCALE * 0.5f;
			else if (diff < 0) SCALE -= SCALE * 0.25f;
			_prevScrlVal = _mouse.GetState().ScrollWheelValue;
			DebugInfo.DebugMessage += $"Spawners: {_spawners.Count}\n";

			int updateVal = (int)MainWindow.Sliders["UpdatePerFrame"].Value;
			if (keyboardState.IsKeyDown(Keys.Space)) return;

			if (updateVal <= 0)
			{
				if (_counter == 0)
					for (int j = _spawners.Count - 1; j >= 0; j--)
						if (_spawners[j].IsDeleted) _spawners.RemoveAt(j);
						else _spawners[j].Update();
				_counter = (_counter + 1) % (-updateVal + 2);
			}
			else
			{
				for (int i = 0; i < updateVal; i++)
					for (int j = _spawners.Count - 1; j >= 0; j--)
						if (_spawners[j].IsDeleted) _spawners.RemoveAt(j);
						else _spawners[j].Update();
			}

			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			if (Win.DrawSwitch.IsChecked ?? false)
			{
				_graphics.GraphicsDevice.Clear(Color.Purple);
				int vx = _mouse.GetState().X;
				int vy = _mouse.GetState().Y;

				World.ViewX = _x;
				World.ViewY = _y;
				World.Draw(vx, vy, _mouse.GetState());
			}
			base.Draw(time);
		}

		public bool SpawnExist(int x, int y)
		{
			var pos = new Vector2(x, y);
			foreach (var ctrl in _spawners)
				if (ctrl.SpawnPos == pos)
					return true;
			return false;
		}

		public void AddSpawn(int x, int y)
		{
			_spawners.Add(new Spawner(x, y, 10, _spawners.Count + 1));
		}

		public void Reset()
		{
			if (World == null) return;

			//_generator = new OpenSimplexNoise()
			//{
			//	Scale = MainWindow.Sliders["Scale"].Value,
			//	Octaves = (int)MainWindow.Sliders["Octaves"].Value,
			//	Persistance = MainWindow.Sliders["Persistance"].Value,
			//	Lacunarity = MainWindow.Sliders["Lacunarity"].Value,
			//	NoiseHeight = MainWindow.Sliders["NoiseHeight"].Value
			//};

			World.Dispose();
			World = new Map(_generator);
		}

		protected override void Dispose(bool disposing)
		{
			SprBatch.Dispose();
			base.Dispose(disposing);
		}
	}
}