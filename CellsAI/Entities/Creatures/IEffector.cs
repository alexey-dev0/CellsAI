namespace CellsAI.Entities.Creatures
{
	public interface IEffector
	{
		double Value { set; }

		void Perform();
	}
}