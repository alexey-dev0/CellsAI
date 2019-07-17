using CellsAI.Entities.Food;
using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Effectors
{
	internal class Absorber : IEffector
	{
		public Absorber(Creature creature) : base(creature)
		{
		}

		public override void Perform()
		{
			if (Value < 0.6) return;

			_creature.GetDirection(out int dx, out int dy);
			int vx = _creature.X + dx;
			int vy = _creature.Y + dy;

			var food = GAME.World[vx, vy].Content.Find(e => e is Eatable);
			if (food != null) _creature.Eat(food as Eatable);
		}
	}
}