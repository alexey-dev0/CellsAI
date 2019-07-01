namespace CellsAI.Entities
{
	public abstract class Entity
	{
		public int X { get; protected set; }
		public int Y { get; protected set; }

		public virtual void Update() { }
	}
}