namespace CellsAI.Entities.Food
{
	public abstract class Eatable : Drawable
	{
		private readonly int _foodValue;

		// [1..100]
		public int FoodValue
		{
			get
			{
				if (!IsDeleted)
					return _foodValue;
				else
					throw new System.Exception();
			}
		}

		public Eatable(int x, int y, int value) : base(x, y)
		{
			_foodValue = value;
		}

		public override string ToString()
		{
			return $"Food {X}, {Y} {IsDeleted}";
		}
	}
}