using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	/// <summary>
	/// The lifebar is used to show an indication of a characters HP
	/// - Goes from 0 to MaHP
	/// - Has a current value
	/// - Health is drawn in a specific color
	/// - Depleted health is drawn in another specific color
	/// - When the character is hit:
	///		- the remaining hp changes color, jumps up and shakes for a second
	///		- The amount of depleted health changes color and jumps off the bar
	/// - When the character is healed:
	///		- the hp changes color and slowly pulsates
	///		- the hp bar quickly grows to the new amount
	/// - When the hp gets to a specified "low amount", it starts flashing between two specified colors
	/// </summary>
	public interface ILifeBar : IMeter
    {
		float CurrentHP { get; set; }

		float MaxHP { get; set; }

		List<Color> HealthColor { get; set; }

		Color DepletedHealthColor { get; set; }

		List<Color> HitHealthColor { get; set; }

		List<Color> HitDepletedHealthColor { get; set; }

		float HealthColorSpeed { get; set; }

		float HitHealthColorSpeed { get; set; }

		float HitHealthDepletedColorSpeed { get; set; }

		float HitTimeDelta { get; set; }

		float HitShakeSpeed { get; set; }

		float HitHealthScaleAmount { get; set; }

		float HitHealthOffsetAmount { get; set; }

		float HitDepletedHealthScaleAmount { get; set; }

		float HitDepletedHealthOffsetAmount { get; set; }

		List<Color> HealColor { get; set; }

		List<Color> HealingColor { get; set; }

		float HealTimeDelta { get; set; }

		float HealScaleAmount { get; set; }

		float HealColorSpeed { get; set; }

		float HealPulsateSpeed { get; set; }

		float NearDeathPercentage { get; set; }

		float NearDeathColorSpeed { get; set; }

		List<Color> NearDeathColors { get; set; }

		void Reset();

		void AddDamage(float damage);

		void Heal(float health);

		void Draw(float currentHealth, IMeterRenderer meterRenderer, SpriteBatch spritebatch);
    }
}
