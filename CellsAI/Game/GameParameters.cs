namespace CellsAI.Game
{
	public static class GameParameters
	{
		public const int CELL_SIZE = 32;
		public const int CHUNK_SIZE = 32;
		public const int CHUNK_FULL_SIZE = CHUNK_SIZE * CELL_SIZE;

		private static float _scale = 0.5f;
		public static float SCALE
		{
			get { return _scale; }
			set { _scale = value <= 0.1f ? 0.1f : value; }
		}

		public static float ZOOM_FACTOR 
			=> 1.0f / (SCALE * CHUNK_SIZE * CELL_SIZE);
	}
}