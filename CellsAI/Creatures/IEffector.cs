namespace CellsAI.Creatures
{
	public interface IEffector
	{
		double Value { set; }

		void Perform();
	}
}
