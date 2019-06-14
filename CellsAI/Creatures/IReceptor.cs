namespace CellsAI.Creatures
{
	public interface IReceptor
	{
		double Value { get; }

		void Receive();
	}
}
