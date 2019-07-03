using System.Collections.Generic;

namespace CellsAI.Entities.Creatures
{
	public interface IReceptor
	{
		List<double> Values { get; }

		void Receive();
	}
}