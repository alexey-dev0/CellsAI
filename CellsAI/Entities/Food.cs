using CellsAI.Game;
using System;

namespace CellsAI.Entities
{
	public class Food : Entity
	{
		// [1..100]
		public int FoodValue { get; }

		public Food(int x, int y, int value = 0)
		{
			X = x;
			Y = y;

			FoodValue = value <= 0 ? new Random(MyGame.Seed).Next(1, 100) : value;
		}
	}
}