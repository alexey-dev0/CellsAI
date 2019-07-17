using CellsAI.Entities.Creatures.Effectors;
using CellsAI.Entities.Creatures.Receptors;
using CellsAI.World;
using Microsoft.Xna.Framework;
using NeuralNetworkLib;

namespace CellsAI.Entities.Creatures
{
	internal class SimpleCreature : Creature
	{
		public SimpleCreature(int x, int y, Spawner controller, NeuralNetwork network = null, int changeCount = 0)
			: base(x, y, controller)
		{
			_receptors.Add(new StraightLook(this));
			_receptors.Add(new NearbyLook(this));
			//_receptors.Add(new LifeIndicator(this));
			_receptors.Add(new DistantLook(this));

			_effectors.Add(new Rotator(this));
			_effectors.Add(new Absorber(this));
			//_effectors.Add(new CellDivisor(this));
			_effectors.Add(new Mover(this));
			_effectors.Add(new Attacker(this));

			if (network != null)
			{
				network = network.Copy();
				network.RandomChange(changeCount, 1);
			}
			_brain = new Brain(_receptors, _effectors, network);
			//_innerColor = GetColor(_brain.GetNetwork().Id);
			//if (_myTexture != null) _myTexture.Dispose();
			//CreateTexture();
			//_texture = _myTexture;
		}

		private Color GetColor(string id)
		{
			if (id.Length < 3) return Color.White;
			int r = 0, g = 0, b = 0;
			for (int i = 0; i < id.Length; i += 3) r += id[i];
			for (int i = 1; i < id.Length; i += 3) g += id[i];
			for (int i = 2; i < id.Length; i += 3) b += id[i];
			return new Color(r % 256, g % 256, b % 256);
		}
	}
}