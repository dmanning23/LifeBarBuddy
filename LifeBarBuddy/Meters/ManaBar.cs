using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	public class ManaBar : Meter, IManaBar
	{
		#region Properties

		private float _currentMP;
		public float CurrentMP
		{
			get
			{
				return _currentMP;
			}
			set
			{
				_currentMP = MathHelper.Clamp(value, 0f, MaxMP);
			}
		}

		private float _maxMP;
		public float MaxMP
		{
			get
			{
				return _maxMP;
			}
			set
			{
				if (value <= 0f)
				{
					throw new Exception($"MaxHP {value} is <= 0");
				}
				_maxMP = value;
			}
		}

		public List<Color> ManaColor { get; set; }
		public Color EmptyManaColor { get; set; }
		public List<Color> InUseManaColor { get; set; }
		public List<Color> InUseDepletedManaColor { get; set; }
		public float ManaColorSpeed { get; set; }
		public float InUseManaColorSpeed { get; set; }
		public float InUseDepletedManaColorSpeed { get; set; }
		public float InUseTimeDelta { get; set; }
		public float InUseShakeSpeed { get; set; }
		public float InUseManaScaleAmount { get; set; }
		public float InUseManaOffsetAmount { get; set; }
		public float InUseDepletedManaScaleAmount { get; set; }
		public float InUseDepletedManaOffsetAmount { get; set; }

		public List<Color> ManaFullColor { get; set; }
		public float ManaFullScaleAmount { get; set; }
		public float ManaFullColorSpeed { get; set; }
		public float ManaFullPulsateSpeed { get; set; }

		private GameClock ManaClock { get; set; }

		private CountdownTimer UseManaTimer { get; set; }

		private float PreUseManaAmount { get; set; }

		private GameClock ManaFullClock { get; set; }

		private bool IsFullManaMode
		{
			get
			{
				return CurrentMP >= MaxMP;
			}
		}

		#endregion //Properties

		#region Methods

		public ManaBar(float maxHP, ContentManager content, string borderImage, string meterImage, string alphaMaskImage, Rectangle position) : 
			base(position)
		{
			ManaClock = new GameClock();
			UseManaTimer = new CountdownTimer();
			ManaFullClock = new GameClock();

			MaxMP = maxHP;
			CurrentMP = 1f;
			ManaColor = new List<Color> { new Color(50, 0, 75), new Color(210, 0, 180) };
			ManaColorSpeed = 1.7f;
			EmptyManaColor = new Color(0.5f, 0.5f, 0.5f);
			InUseManaColor = new List<Color> { new Color(255, 130, 140), new Color(210, 0, 180), new Color(255, 190, 220), new Color(50, 0, 75) };
			InUseDepletedManaColor = new List<Color> { new Color(97, 0, 127), Color.White, new Color(210, 0, 180), new Color(255, 160, 240) };
			InUseManaColorSpeed = 5f;
			InUseDepletedManaColorSpeed = 7f;
			InUseTimeDelta = 0.9f;
			InUseShakeSpeed = 15f;
			InUseManaScaleAmount = 0.08f;
			InUseManaOffsetAmount = 2f;
			InUseDepletedManaScaleAmount = 0.2f;
			InUseDepletedManaOffsetAmount = 1f;

			ManaFullColor = new List<Color> { new Color(210, 0, 180), new Color(255, 130, 140), new Color(50, 0, 75) };
			ManaFullScaleAmount = 0.11f;
			ManaFullColorSpeed = 6f;
			ManaFullPulsateSpeed = 15f;

			LoadContent(content, new Filename(borderImage), new Filename(meterImage), new Filename(alphaMaskImage));
		}

		public void Reset()
		{
			ManaClock.Start();
			UseManaTimer.Stop();
			ManaFullClock.Stop();
			CurrentMP = 0f;
		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			//update all timers
			ManaClock.Update(time);
			UseManaTimer.Update(time);
			ManaFullClock.Update(time);

			UpdateManaFull();
		}

		public override void Update(GameClock time)
		{
			base.Update(time);

			//update all timers
			ManaClock.Update(time);
			UseManaTimer.Update(time);
			ManaFullClock.Update(time);

			UpdateManaFull();
		}

		private void UpdateManaFull()
		{
			//check if we need to change "full energy" mode
			if (ManaFullClock.Paused && IsFullManaMode)
			{
				ManaFullClock.Start();
			}
			else if (!ManaFullClock.Paused && !IsFullManaMode)
			{
				ManaFullClock.Stop();
			}
		}

		public void UseMana(float mana)
		{
			//is the player already taking damage?
			if (!UseManaTimer.HasTimeRemaining)
			{
				PreUseManaAmount = CurrentMP;
			}

			//start the damage timer
			UseManaTimer.Start(InUseTimeDelta);
		}

		public void Draw(float currentHealth, IMeterRenderer meterRenderer, SpriteBatch spritebatch, bool flip = false)
		{
			if (!IsVisible)
			{
				return;
			}

			CurrentMP = currentHealth;

			meterRenderer.DrawBorder(this, spritebatch, Position, Vector2.One, Vector2.Zero, Color.White, flip);

			if (UseManaTimer.HasTimeRemaining)
			{
				//else if the character is being hit, draw in hit mode

				//how much pulsate to add to hp bar?
				var healthPulsate = PulsateScale(UseManaTimer.CurrentTime * InUseShakeSpeed, InUseManaScaleAmount, Position);

				//how much offset to add to health bar?
				var healthOffset = OffsetVector(UseManaTimer.CurrentTime * InUseShakeSpeed, InUseManaOffsetAmount);

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxMP, CurrentMP);

				//what is the alpha of the current HP?
				var damageAlpha = ConvertToAlpha(0, MaxMP, PreUseManaAmount);

				//how much pulsate to add to damage bar?
				var damagePulsate = PulsateScale(UseManaTimer.CurrentTime * InUseShakeSpeed, InUseDepletedManaScaleAmount, Position);

				//how much offset to add to health bar?
				var damageOffset = OffsetVector(UseManaTimer.CurrentTime * InUseShakeSpeed, InUseDepletedManaOffsetAmount);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, Position, currentAlpha, 1f, Vector2.One, healthOffset, EmptyManaColor, flip);

				//draw the damage bar
				meterRenderer.DrawMeter(this, spritebatch, Position, currentAlpha, damageAlpha, damagePulsate, damageOffset, GetInUseManaDepletedColor(), flip);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, Position, 0f, currentAlpha, healthPulsate, healthOffset, GetInUseManaColor(), flip);
			}
			else if (!ManaFullClock.Paused)
			{
				//else if the energy is full, draw in FullEnergy mode

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(ManaFullClock.CurrentTime * ManaFullPulsateSpeed, ManaFullScaleAmount, Position);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, Position, 0f, 1f, pulsate, Vector2.Zero, GetManaFullColor(), flip);
			}
			else
			{
				//otherwise draw the health bar normal

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxMP, CurrentMP);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, Position, currentAlpha, 1f, Vector2.One, Vector2.Zero, EmptyManaColor, flip);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, Position, 0f, currentAlpha, Vector2.One, Vector2.Zero, GetManaColor(), flip);
			}
		}

		private Color GetManaColor()
		{
			return LerpColors(ManaClock.CurrentTime * ManaColorSpeed, ManaColor);
		}

		private Color GetInUseManaColor()
		{
			return LerpColors(UseManaTimer.CurrentTime * InUseManaColorSpeed, InUseManaColor);
		}

		private Color GetInUseManaDepletedColor()
		{
			var color = LerpColors(UseManaTimer.CurrentTime * InUseDepletedManaColorSpeed, InUseDepletedManaColor);
			color.A = (byte)(UseManaTimer.Lerp * 255);

			return color;
		}

		private Color GetManaFullColor()
		{
			return LerpColors(ManaFullClock.CurrentTime * ManaFullColorSpeed, ManaFullColor);
		}

		#endregion //Methods
	}
}
