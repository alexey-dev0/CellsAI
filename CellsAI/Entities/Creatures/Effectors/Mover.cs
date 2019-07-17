using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Mover : IEffector
	{
		public Mover(Creature creature) : base(creature)
		{
		}

		public override void Perform()
		{
			if (Value < 0.6) return;
			_creature.GetDirection(out int dx, out int dy);
			if (!GAME.World[_creature.X + dx, _creature.Y + dy].Content.Exists(e => e is Creature))
				_creature.Move(dx, dy);
		}
	}
}