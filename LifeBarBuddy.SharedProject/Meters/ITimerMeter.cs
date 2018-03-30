using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	public interface ITimerMeter : IMeter
	{
		float CurrentTime { get; set; }

		float MaxTime { get; set; }

		List<Color> TimeColors { get; set; }

		Color DepletedTimeColor { get; set; }

		float TimeColorSpeed { get; set; }

		float NearEndTime { get; set; }

		float NearEndColorSpeed { get; set; }

		float NearEndPulsateSpeed { get; set; }

		float NearEndPulsateAmount { get; set; }

		List<Color> NearEndColors { get; set; }

		float TimeOutShakeSpeed { get; set; }

		float TimeOutShakeScale { get; set; }

		List<Color> TimeOutColors { get; set; }
		float TimeOutColorSpeed { get; set; }

		void Reset();

		void Draw(float currentTime, IMeterRenderer meterRenderer, SpriteBatch spritebatch);
	}
}
