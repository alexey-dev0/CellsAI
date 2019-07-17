namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Rotator : IEffector
	{
		public Rotator(Creature creature) : base(creature)
		{
		}

		public override void Perform()
		{
			if (Value <= 0.4) _creature.MyRotation--;
			else if (Value >= 0.6) _creature.MyRotation++;
			_creature.MyRotation = (Rotation)((int)(_creature.MyRotation + 8) % 8);
		}
	}
}