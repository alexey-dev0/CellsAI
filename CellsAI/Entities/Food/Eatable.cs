using CellsAI.Game;

namespace CellsAI.Entities.Food
{
	public abstract class Eatable : Drawable
	{
		private int _foodValue;
		private bool _deleted;

		// [1..100]
		public int FoodValue {
			get
			{
				//if (!_deleted)
					return _foodValue;
				//else
				//	throw new System.Exception();
			}
		}

		public Eatable(int x, int y, int value = 0)
		{
			X = x;
			Y = y;

			_foodValue = 100;//value <= 0 ? new Random(MyGame.Seed).Next(1, 100) : value;
		}

		public override string ToString()
		{
			return $"Food {X}, {Y}";
		}

		public void Delete()
		{
			MyGame.World[X, Y].Leave(this);
			_deleted = true;
		}
	}
}