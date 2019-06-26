using CellsAI.Game;
using System;

namespace CellsAI.Entities.Creatures.Receptors
{
	public class StraightLook : IReceptor
	{
		private int VX => MyGame.World.ViewX;

		public double Value => throw new NotImplementedException();

		public void Receive()
		{
			throw new NotImplementedException();
		}
	}
}