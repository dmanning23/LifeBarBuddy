using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBarBuddy
{
	/// <summary>
	/// The Super Bar is used to track energy to be used for special attacks.
	/// - The super bar is displayed in one color.
	/// - As energy is added, if it fills a stock, that stock will pulsate for a second.
	/// - Filled stocks flash between two colors.
	/// - When a stock is used, the last filled stock changes color and jumps off the meter.
	/// - When the super bar is completely full, the whole thing starts pulsating
	/// </summary>
	public interface IStockSuperBar
	{
		float CurrentEnergy { get; set; }

		float MaxEnergy { get; set; }

		int StockAmount { get; set; }

		Color EnergyColor { get; set; }

		Color EmptyColor { get; set; }

		float StockUpTimeDelta { get; set; }

		float StockUpScaleAmount { get; set; }

		List<Color> StockColors { get; set; }

		Color StockUsedColor { get; set; }

		float StockUsedTimeDelta { get; set; }

		float StockUsedScaleAmount { get; set; }

		float StockUsedOffsetAmount { get; set; }

		float StockFullScaleAmount { get; set; }

		float StockFullOffsetAmount { get; set; }

		void Reset();

		void AddEnergy(float energy);

		void UseStock(int stock);

		void Draw();
	}
}
