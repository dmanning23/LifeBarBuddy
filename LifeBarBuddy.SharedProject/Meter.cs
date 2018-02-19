using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	public abstract class Meter : IMeter
	{
		#region Properties

		public bool HorizontalMeter { get; set; }

		public Texture2D BorderImage { get; set; }

		public Texture2D MeterImage { get; set; }

		public Texture2D AlphaMaskImage { get; set; }

		protected GameClock Clock { get; set; }

		#endregion //Properties

		#region Methods

		public Meter()
		{
			HorizontalMeter = true;
			Clock = new GameClock();
		}

		public virtual void Update(GameTime time)
		{
			Clock.Update(time);
		}

		protected void LoadContent(ContentManager content, Filename borderImage, Filename meterImage, Filename alphaMaskImage)
		{
			BorderImage = content.Load<Texture2D>(borderImage.GetRelPathFileNoExt());
			MeterImage = content.Load<Texture2D>(meterImage.GetRelPathFileNoExt());
			AlphaMaskImage = content.Load<Texture2D>(alphaMaskImage.GetRelPathFileNoExt());
		}

		protected Vector2 PulsateScale(float currentTime, float size, Rectangle rect)
		{
			//the full pulsate amount
			var pulsate = Pulsate(currentTime, size);

			//which side needs to pulsate less?
			if (rect.Height < rect.Width)
			{
				var aspect = ((float)rect.Height / (float)rect.Width);
				var halfPulsate = Pulsate(currentTime, size * aspect);
				return new Vector2(halfPulsate, pulsate);
			}
			else
			{
				var halfPulsate = Pulsate(currentTime, size * (rect.Width / rect.Height));
				return new Vector2(pulsate, halfPulsate);
			}
		}

		private float Pulsate(float currentTime, float size)
		{
			//Pulsate the size of the text, bump it up so it starts at 1
			return (size * (float)(Math.Cos(currentTime))) + 1 + size;
		}

		protected Vector2 OffsetVector(float currentTime, float offsetAmount)
		{
			return new Vector2((float)Math.Sin(currentTime) * offsetAmount, (float)Math.Cos(currentTime) * offsetAmount);
		}

		protected Color LerpColors(float currentTime, List<Color> colors)
		{
			var index = (int)currentTime % colors.Count;

			//get the next color
			var nextIndex = index + 1;
			if (nextIndex >= colors.Count)
			{
				nextIndex = 0;
			}

			//get the ACTUAL lerped color of this letter
			var remainder = (float)(currentTime - (int)currentTime);
			return Color.Lerp(colors[index], colors[nextIndex], remainder);
		}

		protected Vector2 LerpScale(CountdownTimer currentTime, float size, Rectangle rect)
		{
			var scale = GetLerpScale(currentTime, size);

			//which side needs to pulsate less?
			if (rect.Height < rect.Width)
			{
				var aspect = ((float)rect.Height / (float)rect.Width);
				var halfScale = GetLerpScale(currentTime, 1 + (size * aspect));
				return new Vector2(halfScale, scale);
			}
			else
			{
				var halfScale = GetLerpScale(currentTime, size * (rect.Width / rect.Height));
				return new Vector2(scale, halfScale);
			}
		}

		private float GetLerpScale(CountdownTimer currentTime, float size)
		{
			return 1f + ((size - 1f) * (1f - currentTime.Lerp));
		}

		protected Color FadeColor(CountdownTimer currentTime, Color startColor)
		{
			var shadow = startColor;
			shadow.A =(byte)((float)startColor.A * currentTime.Lerp);
			return shadow;
		}

		protected float ConvertToAlpha(float start, float end, float current)
		{
			return ((current - start) / (end - start));
		}

		#endregion //Methods
	}
}
