namespace CellsAI.Entities.Creatures.Effectors
{
	class Mover : IEffector
	{
		private Creature _creature;

		public double Value { get; set; }

		public Mover(Creature creature)
		{
			_creature = creature;
		}

		public void Perform()
		{
			if (Value < 0.9) return;
			int dx = 0, dy = 0;
			switch (_creature.MyRotation)
			{
				case Creature.Rotation.Right:
					dx = 1;
					break;
				case Creature.Rotation.Down:
					dy = 1;
					break;
				case Creature.Rotation.Left:
					dx = -1;
					break;
				case Creature.Rotation.Up:
					dy = -1;
					break;
			}

			_creature.Move(dx, dy);
		}
	}
}
