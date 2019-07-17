using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities.Creatures.Effectors
{
	internal class CellDivisor : IEffector
	{
		public CellDivisor(Creature creature) : base(creature)
		{
		}

		public override void Perform()
		{
			if (Value < 0.9) return;
			if (_creature.Health / _creature.MaxHealth < 0.9) return;

			var x = _creature.X;
			var y = _creature.Y;

			int dx, dy = 0;
			bool empty = false;
			for (dx = -1; dx <= 1 && !empty; dx++)
				for (dy = -1; dy <= 1 && !empty; dy++)
					if (GAME.World[x + dx, y + dy].Content.Count == 0)
						empty = true;
			if (!empty) return;

			var network = _creature.GetNetwork();

			var creature1 = new SimpleCreature(x, y, _creature.MySpawner, network, 2)
			{
				Health = _creature.Health / 2,
				Lifetime = _creature.Lifetime
			};
			var creature2 = new SimpleCreature(x + dx, y + dy, _creature.MySpawner, network, 5)
			{
				Health = _creature.Health / 2,
				Lifetime = _creature.Lifetime
			};

			GAME.World[x, y].Enter(creature1);
			GAME.World[x + dx, y + dy].Enter(creature2);

			_creature.MySpawner.AddNewCreatures(creature1, creature2);
			_creature.Health = 0;
		}
	}
}