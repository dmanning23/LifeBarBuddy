using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeBarBuddy
{
	public interface IMeter
	{
		Rectangle Position { get; set; }

		bool HorizontalMeter { get; set; }

		Texture2D BorderImage { get; set; }

		Texture2D MeterImage { get; set; }

		Texture2D AlphaMaskImage { get; set; }

		void Update(GameTime time);

		void Update(GameClock time);
	}
}
