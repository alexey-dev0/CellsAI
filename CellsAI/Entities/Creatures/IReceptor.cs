namespace CellsAI.Entities.Creatures
{
	public interface IReceptor
	{
		double Value { get; }

		void Receive();
	}
}