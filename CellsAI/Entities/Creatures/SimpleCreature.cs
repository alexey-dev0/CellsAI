using Microsoft.Xna.Framework.Graphics;
using NeuralNetworkLib;

namespace CellsAI.Entities.Creatures
{
	internal class SimpleCreature : Creature
	{
		public SimpleCreature(SpriteBatch sprBatch, int x, int y) : base(sprBatch)
		{
			// ... receptors & effectors init in child object;
			X = x;
			Y = y;
			var network = new MultilayerPerceptron(_receptors.Count, _effectors.Count, ActivationFunctions.Sigmoid);
			_brain = new Brain(_receptors, _effectors, network);
		}
	}
}