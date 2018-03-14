using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	public class TimerMeter : Meter, ITimerMeter
	{
		#region Properties

		private float _currentTime;
		public float CurrentTime
		{
			get
			{
				return _currentTime;
			}
			set
			{
				_currentTime = MathHelper.Clamp(value, 0f, MaxTime);
			}
		}

		private float _maxTime;
		public float MaxTime
		{
			get
			{
				return _maxTime;
			}
			set
			{
				if (value <= 0f)
				{
					throw new Exception($"MaxTime {value} is <= 0");
				}
				_maxTime = value;
			}
		}

		public List<Color> TimeColors { get; set; }
		public Color DepletedTimeColor { get; set; }
		public float TimeColorSpeed { get; set; }

		public float NearEndTime { get; set; }
		public float NearEndColorSpeed { get; set; }
		public float NearEndPulsateSpeed { get; set; }
		public float NearEndPulsateAmount { get; set; }
		public List<Color> NearEndColors { get; set; }

		private float TimeOutFade { get; set; }
		public float TimeOutShakeSpeed { get; set; }
		public float TimeOutShakeScale { get; set; }
		public List<Color> TimeOutColors { get; set; }
		public float TimeOutColorSpeed { get; set; }

		private GameClock TimeClock { get; set; }

		private bool IsNearEndMode
		{
			get
			{
				return CurrentTime <= NearEndTime && CurrentTime > 0;
			}
		}

		#endregion //Properties

		#region Methods

		public TimerMeter(float maxTime, ContentManager content, string borderImage, string meterImage, string alphaMaskImage, Rectangle position) :
			base(position)
		{
			TimeClock = new GameClock();

			MaxTime = maxTime;
			CurrentTime = MaxTime;

			TimeColors = new List<Color> { new Color(80, 0, 0), new Color(240, 0, 0) };
			DepletedTimeColor = new Color(0.8f, 0.8f, 0.8f);
			TimeColorSpeed = 1f;

			NearEndTime = MaxTime * 0.4f;
			NearEndColorSpeed = 10f;
			NearEndPulsateSpeed = 15f;
			NearEndPulsateAmount = 0.11f;
			NearEndColors = new List<Color> { Color.Red, new Color(0.4f, 0f, 0f), Color.HotPink };

			TimeOutShakeSpeed = 50f;
			TimeOutShakeScale = 8f;
			TimeOutColors = new List<Color> { Color.Red, new Color(0.9f, 0.8f, 0f), new Color(0.4f, 0f, 0f), DepletedTimeColor };
			TimeOutColorSpeed = 5f;

			LoadContent(content, new Filename(borderImage), new Filename(meterImage), new Filename(alphaMaskImage));
		}

		public void Reset()
		{
			TimeClock.Start();
			CurrentTime = MaxTime;
		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			//update all timers
			TimeClock.Update(time);
		}

		public override void Update(GameClock time)
		{
			base.Update(time);

			//update all timers
			TimeClock.Update(time);
		}

		public void Draw(float currentTime, IMeterRenderer meterRenderer, SpriteBatch spritebatch)
		{
			if (!IsVisible)
			{
				return;
			}

			CurrentTime = currentTime;

			var correctedPosition = GetShakyRectangle();

			if (IsNearEndMode)
			{
				//else if the character is near-death, draw in near-death mode

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxTime, CurrentTime);

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(TimeClock.CurrentTime * NearEndPulsateSpeed, NearEndPulsateAmount, correctedPosition);

				//draw the border
				meterRenderer.DrawBorder(this, spritebatch, correctedPosition, pulsate, Vector2.Zero, Color.White);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, correctedPosition, 0f, currentAlpha, pulsate, Vector2.Zero, DepletedTimeColor);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, correctedPosition, currentAlpha, 1f, pulsate, Vector2.Zero, GetNearEndTimeColor());
			}
			else if (CurrentTime <= 0f)
			{
				//otherwise draw the health bar normal

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxTime, CurrentTime);

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(TimeClock.CurrentTime * NearEndPulsateSpeed, NearEndPulsateAmount * 0.5f, correctedPosition);

				//draw the border
				meterRenderer.DrawBorder(this, spritebatch, correctedPosition, Vector2.One, Vector2.Zero, Color.White);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, correctedPosition, 0f, 1f, pulsate, Vector2.Zero, GetTimeOutColor());
			}
			else
			{
				//otherwise draw the health bar normal

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxTime, CurrentTime);

				//draw the border
				meterRenderer.DrawBorder(this, spritebatch, correctedPosition, Vector2.One, Vector2.Zero, Color.White);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, correctedPosition, 0f, currentAlpha, Vector2.One, Vector2.Zero, DepletedTimeColor);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, correctedPosition, currentAlpha, 1f, Vector2.One, Vector2.Zero, GetTimeColor());
			}
		}

		private Color GetTimeColor()
		{
			return LerpColors(TimeClock.CurrentTime * TimeColorSpeed, TimeColors);
		}

		private Color GetNearEndTimeColor()
		{
			return LerpColors(TimeClock.CurrentTime * NearEndColorSpeed, NearEndColors);
		}

		private Color GetTimeOutColor()
		{
			return LerpColors(TimeClock.CurrentTime * TimeOutColorSpeed, TimeOutColors);
		}

		private Rectangle GetShakyRectangle()
		{
			if (CurrentTime <= 0)
			{
				TimeOutFade = Math.Min(TimeOutFade + (TimeClock.TimeDelta * TimeOutShakeSpeed), 1f);
			}
			else
			{
				TimeOutFade = Math.Max(TimeOutFade - (TimeClock.TimeDelta * TimeOutShakeSpeed), 0f);
			}

			//Pulsate the size of the text
			var pulsate = (TimeOutShakeScale * (float)(Math.Sin(TimeClock.CurrentTime * TimeOutShakeSpeed)));
			pulsate *= TimeOutFade;

			return new Rectangle((int)(Position.X - pulsate), Position.Y, Position.Width, Position.Height);
		}

		#endregion //Methods
	}
}
