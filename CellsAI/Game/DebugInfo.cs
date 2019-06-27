using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;

namespace CellsAI.Game
{
	public class DebugInfo : WpfDrawableGameComponent
	{
		private SpriteBatch _spriteBatch;
		private int _frames;
		private int _liveFrames;
		private TimeSpan _timeElapsed;

		public static SpriteFont DefaultFont;
		public static string DebugMessage { get; set; } = "";

		public DebugInfo(WpfGame game) : base(game)
		{
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
			DebugMessage += $"FPS: {_frames}\n";
			_spriteBatch.DrawString(DefaultFont, DebugMessage, new Vector2(6), Color.Black);
			_spriteBatch.DrawString(DefaultFont, DebugMessage, new Vector2(5), Color.OrangeRed);
			DebugMessage = "";
			_spriteBatch.End();
		}
	}
}