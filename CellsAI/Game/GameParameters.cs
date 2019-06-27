namespace CellsAI.Game
{
	public static class GameParameters
	{
		public const int CELL_SIZE = 16;
		public const int CHUNK_SIZE = 16;
		public const int CHUNK_FULL_SIZE = CHUNK_SIZE * CELL_SIZE;

		private static float _scale = 1;
		public static float SCALE
		{
			get { return _scale; }
			set { _scale = value <= 0.1f ? 0.1f : value; }
		}

		public static double ZOOM_FACTOR 
			=> 1.0 / (SCALE * CHUNK_SIZE * CELL_SIZE);
	}
}