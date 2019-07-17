using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;

namespace CellsAI.Game
{
	public class DebugInfo : WpfDrawableGameComponent, IDisposable
	{
		private SpriteBatch _spriteBatch;
		private int _frames;
		private int _liveFrames;
		private TimeSpan _timeElapsed;
		private readonly MyGame _game;

		public static SpriteFont DefaultFont;
		public static string DebugMessage { get; set; } = "";

		public DebugInfo(WpfGame game) : base(game)
		{
			_game = game as MyGame;
		}

		protected override void LoadContent()
		{
			Game.Content.RootDirectory = "Assets";
			DefaultFont = Game.Content.Load<SpriteFont>("Fonts/DefaultFont");

			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		public override void Update(GameTime gameTime)
		{
			_timeElapsed += gameTime.ElapsedGameTime;
			if (_timeElapsed >= TimeSpan.FromSeconds(1))
			{
				_timeElapsed -= TimeSpan.FromSeconds(1);
				_frames = _liveFrames;
				_liveFrames = 0;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			_liveFrames++;
			_spriteBatch.Begin();
			DebugMessage = $"FPS: {_frames}\n" + DebugMessage;
			_game.Win.DebugInfo.Text = DebugMessage;
			//_spriteBatch.DrawString(DefaultFont, DebugMessage, new Vector2(6), Color.Black);
			//_spriteBatch.DrawString(DefaultFont, DebugMessage, new Vector2(5), Color.White);
			DebugMessage = "";
			_spriteBatch.End();
		}

		public void Dispose()
		{
			if (!_spriteBatch.IsDisposed) _spriteBatch.Dispose();
		}
	}
}