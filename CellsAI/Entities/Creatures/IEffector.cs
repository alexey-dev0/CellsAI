namespace CellsAI.Entities.Creatures
{
	public interface IEffector
	{
		double Value { get;  set; }

		void Perform();
	}
}