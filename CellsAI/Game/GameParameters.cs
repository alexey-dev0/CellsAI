namespace CellsAI.Game
{
	public static class GameParameters
	{
		#region View

		public const int CELL_SIZE = 64;
		public const int CHUNK_SIZE = 32;
		private static float _scale = 0.05f;

		public static float SCALE
		{
			get { return _scale; }
			set { _scale = value <= 0.05f ? 0.05f : value; }
		}

		public static float ZOOM_FACTOR
			=> SCALE * CHUNK_SIZE * CELL_SIZE;

		#endregion View

		#region Game

		public static MyGame GAME;
		public const int ISLAND_SIZE = 10000;

		#endregion Game
	}
}