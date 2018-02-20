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

		List<Color> UseEnergyColor { get; set; }

		List<Color> UseEnergyDepletedColor { get; set; }

		Color UseEnergyShadowColor { get; set; }

		float UseEnergyShadowTimeDelta { get; set; }

		float UseEnergyShadowTargetScale { get; set; }

		float UseEnergyTimeDelta { get; set; }

		float UseEnergyColorSpeed { get; set; }

		float UseEnergyDepletedColorSpeed { get; set; }

		float UseEnergyShakeAmount { get; set; }

		float UseEnergyShakeSpeed { get; set; }

		void Reset();

		void AddEnergy(float energy);

		void UseEnergy();

		void Draw(float currentEnergy, IMeterRenderer meterRenderer, SpriteBatch spritebatch);

	}
}
