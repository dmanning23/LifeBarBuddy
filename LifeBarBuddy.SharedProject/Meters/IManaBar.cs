using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	/// <summary>
	/// The mana bar is used to show an indication of a characters MP
	/// - Starts at 0
	/// - Goes from 0 to MaMP
	/// - Has a current value
	/// - Mana is drawn in a rotating color
	/// - Empty bar health is drawn in a specific color
	/// - Full Mana Bar is drawn in flashing color (to indicate full)
	/// - When the mana is used:
	///		- the remaining MP changes color, jumps up and shakes for a second
	///		- The amount of depleted mana changes color and jumps off the bar
	/// </summary>
	public interface IManaBar : IMeter
    {
		float CurrentMP { get; set; }

		float MaxMP { get; set; }

		List<Color> ManaColor { get; set; }

		Color EmptyManaColor { get; set; }

		List<Color> InUseManaColor { get; set; }

		List<Color> InUseDepletedManaColor { get; set; }

		float ManaColorSpeed { get; set; }

		float InUseManaColorSpeed { get; set; }

		float InUseDepletedManaColorSpeed { get; set; }

		float InUseTimeDelta { get; set; }

		float InUseShakeSpeed { get; set; }

		float InUseManaScaleAmount { get; set; }

		float InUseManaOffsetAmount { get; set; }

		float InUseDepletedManaScaleAmount { get; set; }

		float InUseDepletedManaOffsetAmount { get; set; }

		List<Color> ManaFullColor { get; set; }

		float ManaFullScaleAmount { get; set; }

		float ManaFullColorSpeed { get; set; }

		float ManaFullPulsateSpeed { get; set; }

		void Reset();

		void UseMana(float mana);

		void Draw(float currentMana, IMeterRenderer meterRenderer, SpriteBatch spritebatch, bool flip = false);
    }
}
