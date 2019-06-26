using CellsAI.Game;
using System;

namespace CellsAI.Entities
{
	public class Food : Entity
	{
		private int _x;
		public int X => _x;

		private int _y;
		public int Y => _y;

		// [1..100]
		public int FoodValue { get; }

		public Food(int x, int y, int value = 0)
		{
			_x = x;
			_y = y;

			FoodValue = value <= 0 ? new Random(MyGame.Seed).Next(1, 100) : value;
		}
	}
}