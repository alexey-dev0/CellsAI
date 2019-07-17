using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Attacker : IEffector
	{
		public Attacker(Creature creature) : base(creature)
		{
		}

		public override void Perform()
		{
			if (Value < 0.8) return;
			_creature.GetDirection(out int dx, out int dy);
			var other = GAME.World[_creature.X + dx, _creature.Y + dy].Content.Find(e => e is Creature) as Creature;
			if (other != null && other.MyColor != _creature.MyColor)
				_creature.Attack(other);
		}
	}
}