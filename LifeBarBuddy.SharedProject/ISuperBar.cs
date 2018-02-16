using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBarBuddy
{
	/// <summary>
	/// This meter starts at zero, energy is added to is
	/// When it hits the max, it starts flashing to indicate the super is ready
	/// </summary>
	public interface ISuperBar : IMeter
	{
		float CurrentEnergy { get; set; }

		float MaxEnergy { get; set; }

		List<Color> EnergyColor { get; set; }

		Color EmptyEnergyColor { get; set; }

		float EnergyColorSpeed { get; set; }

		List<Color> AddEnergyColor { get; set; }

		List<Color> AddingEnergyColor { get; set; }

		float AddEnergyTimeDelta { get; set; }

		float AddEnergyScaleAmount { get; set; }

		float AddEnergyColorSpeed { get; set; }

		float AddEnergyPulsateSpeed { get; set; }

		List<Color> EnergyFullColor { get; set; }

		float EnergyFullScaleAmount { get; set; }

		float EnergyFullColorSpeed { get; set; }

		float EnergyFullPulsateSpeed { get; set; }

		void Reset();

		void AddEnergy(float energy);

		void Draw(float currentEnergy, IMeterRenderer meterRenderer, SpriteBatch spritebatch, Rectangle position);

	}
}
