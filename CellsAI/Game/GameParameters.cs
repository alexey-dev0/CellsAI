namespace CellsAI.Game
{
	public static class GameParameters
	{
		#region View
		public const int CELL_SIZE = 32;
		public const int CHUNK_SIZE = 32;
		private static float _scale = 0.5f;

		public static float SCALE
		{
			get { return _scale; }
			set { _scale = value <= 0.1f ? 0.1f : value; }
		}

		public static float ZOOM_FACTOR
			=> SCALE * CHUNK_SIZE * CELL_SIZE;
		#endregion
	}
}