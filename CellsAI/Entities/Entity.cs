using static CellsAI.Game.GameParameters;

namespace CellsAI.Entities
{
	public abstract class Entity
	{
		public bool IsDeleted { get; protected set; }

		private int _x;

		public int X
		{
			get
			{
				if (!IsDeleted)
					return _x;
				else
					throw new System.Exception();
			}
			protected set
			{
				GAME.World[_x, _y].Leave(this);
				_x = value;
				GAME.World[_x, _y].Enter(this);
			}
		}

		private int _y;

		public int Y
		{
			get
			{
				if (!IsDeleted)
					return _y;
				else
					throw new System.Exception();
			}
			protected set
			{
				GAME.World[_x, _y].Leave(this);
				_y = value;
				GAME.World[_x, _y].Enter(this);
			}
		}

		protected Entity(int x, int y)
		{
			_x = x;
			_y = y;
			GAME.World[x, y].Enter(this);
		}

		public virtual void Update()
		{
		}

		public virtual void Delete()
		{
			GAME.World[X, Y].Leave(this);
			IsDeleted = true;
		}
	}
}