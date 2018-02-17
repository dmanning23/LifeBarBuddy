using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LifeBarBuddy
{
	public class LifeBar : Meter, ILifeBar
	{
		#region Properties

		private float _currentHP;
		public float CurrentHP
		{
			get
			{
				return _currentHP;
			}
			set
			{
				_currentHP = MathHelper.Clamp(value, 0f, MaxHP);
			}
		}

		private float _maxHP;
		public float MaxHP
		{
			get
			{
				return _maxHP;
			}
			set
			{
				if (value <= 0f)
				{
					throw new Exception($"MaxHP {value} is <= 0");
				}
				_maxHP = value;
			}
		}

		public List<Color> HealthColor { get; set; }
		public Color DepletedHealthColor { get; set; }
		public List<Color> HitHealthColor { get; set; }
		public List<Color> HitDepletedHealthColor { get; set; }
		public float HealthColorSpeed { get; set; }
		public float HitHealthColorSpeed { get; set; }
		public float HitHealthDepletedColorSpeed { get; set; }
		public float HitTimeDelta { get; set; }
		public float HitShakeSpeed { get; set; }
		public float HitHealthScaleAmount { get; set; }
		public float HitHealthOffsetAmount { get; set; }
		public float HitDepletedHealthScaleAmount { get; set; }
		public float HitDepletedHealthOffsetAmount { get; set; }
		public List<Color> HealColor { get; set; }
		public List<Color> HealingColor { get; set; }
		public float HealTimeDelta { get; set; }
		public float HealScaleAmount { get; set; }
		public float HealColorSpeed { get; set; }
		public float HealPulsateSpeed { get; set; }
		public float NearDeathPercentage { get; set; }
		public float NearDeathColorSpeed { get; set; }
		public List<Color> NearDeathColors { get; set; }

		private GameClock HealthClock { get; set; }

		private CountdownTimer HitTimer { get; set; }

		private CountdownTimer HealTimer { get; set; }

		private GameClock NearDeathClock { get; set; }

		private bool IsLowHealthMode
		{
			get
			{
				return CurrentHP <= (MaxHP * NearDeathPercentage);
			}
		}

		private float PreHealAmount { get; set; }

		private float PreDamageAmount { get; set; }

		#endregion //Properties

		#region Methods

		public LifeBar(float maxHP, ContentManager content, Filename borderImage, Filename meterImage, Filename alphaMaskImage)
		{
			HealthClock = new GameClock();
			HitTimer = new CountdownTimer();
			HealTimer = new CountdownTimer();
			NearDeathClock = new GameClock();

			MaxHP = maxHP;
			CurrentHP = 1f;
			HealthColor = new List<Color> { new Color(100, 0,0), new Color(180, 0, 0) };
			DepletedHealthColor = new Color(0.5f, 0.5f, 0.5f);
			HitHealthColor = new List<Color> { Color.DarkRed, Color.HotPink };
			HitDepletedHealthColor = new List<Color> { new Color(0f,0f,0f, 0.5f), Color.Yellow };
			HitHealthColorSpeed = 5f;
			HitHealthDepletedColorSpeed = 7f;
			HealthColorSpeed = 0.75f;
			HitTimeDelta = 0.8f;
			HitShakeSpeed = 30f;
			HitHealthScaleAmount = 0.025f;
			HitHealthOffsetAmount = 1f;
			HitDepletedHealthScaleAmount = 0.08f;
			HitDepletedHealthOffsetAmount = 2f;
			HealColor = new List<Color> { Color.Green, Color.LightGreen };
			HealingColor = new List<Color> { Color.DarkGreen, Color.Green };
			HealTimeDelta = 1f;
			HealScaleAmount = 0.04f;
			HealPulsateSpeed = 20f;
			HealColorSpeed = 5f;
			NearDeathPercentage = .2f;
			NearDeathColorSpeed = 10f;
			NearDeathColors = new List<Color> { Color.Red, new Color (0.4f, 0f,0f) };

			LoadContent(content, borderImage, meterImage, alphaMaskImage);
		}

		public void Reset()
		{
			HealthClock.Start();
			HitTimer.Stop();
			HealTimer.Stop();
			NearDeathClock.Stop();
			CurrentHP = MaxHP;
		}

		public override void Update(GameClock time)
		{
			base.Update(time);

			//update all timers
			HealthClock.Update(time);
			HitTimer.Update(time);
			HealTimer.Update(time);
			NearDeathClock.Update(time);

			//check if we need to change "low health" mode
			if (NearDeathClock.Paused && IsLowHealthMode)
			{
				NearDeathClock.Start();
			}
			else if (!NearDeathClock.Paused && !IsLowHealthMode)
			{
				NearDeathClock.Stop();
			}
		}

		public void Heal(float health)
		{
			//is the player already being healed?
			if (!HealTimer.HasTimeRemaining)
			{
				PreHealAmount = CurrentHP;
			}
			else
			{
				PreHealAmount = PreHealAmount + ((CurrentHP - PreHealAmount) * (1 - HealTimer.Lerp));
			}

			//start the healing timer
			HealTimer.Start(HealTimeDelta);
		}

		public void AddDamage(float damage)
		{
			//is the player already taking damage?
			if (!HitTimer.HasTimeRemaining)
			{
				PreDamageAmount = CurrentHP;
			}

			//start the damage timer
			HitTimer.Start(HitTimeDelta);
		}

		public void Draw(float currentHealth, IMeterRenderer meterRenderer, SpriteBatch spritebatch, Rectangle position)
		{
			CurrentHP = currentHealth;

			meterRenderer.DrawBorder(this, spritebatch, position, Vector2.One, Vector2.Zero, Color.White);

			if (HealTimer.HasTimeRemaining)
			{
				//If the character is being healed, draw in heal mode

				//how much health bar to draw?
				var healingHealthBar = PreHealAmount + ((CurrentHP - PreHealAmount) * (1f - HealTimer.Lerp));
				var healingAlpha = ConvertToAlpha(0, MaxHP, healingHealthBar);

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(HealTimer.CurrentTime * HealPulsateSpeed, HealScaleAmount, position);

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxHP, CurrentHP);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, 1f, Vector2.One, Vector2.Zero, DepletedHealthColor);

				//draw the healing part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, healingAlpha, currentAlpha, Vector2.One, Vector2.Zero, GetHealingColor());

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, healingAlpha, Vector2.One, Vector2.Zero, GetHealColor());
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, healingAlpha, pulsate, Vector2.Zero, GetHealColor());
			}
			else if (HitTimer.HasTimeRemaining)
			{
				//else if the character is being hit, draw in hit mode

				//how much pulsate to add to hp bar?
				var healthPulsate = PulsateScale(HitTimer.CurrentTime * HitShakeSpeed, HitHealthScaleAmount, position);

				//how much offset to add to health bar?
				var healthOffset = OffsetVector(HitTimer.CurrentTime * HitShakeSpeed, HitHealthOffsetAmount);

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxHP, CurrentHP);

				//what is the alpha of the current HP?
				var damageAlpha = ConvertToAlpha(0, MaxHP, PreDamageAmount);

				//how much pulsate to add to damage bar?
				var damagePulsate = PulsateScale(HitTimer.CurrentTime * HitShakeSpeed, HitDepletedHealthScaleAmount, position);

				//how much offset to add to health bar?
				var damageOffset = OffsetVector(HitTimer.CurrentTime * HitShakeSpeed, HitDepletedHealthOffsetAmount);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, currentAlpha, 1f, Vector2.One, Vector2.Zero, DepletedHealthColor);

				//draw the damage bar
				meterRenderer.DrawMeter(this, spritebatch, position, currentAlpha, damageAlpha, damagePulsate, damageOffset, GetHitDepletedHealthColor());

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, currentAlpha, Vector2.One, Vector2.Zero, GetHitHealthColor());
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, currentAlpha, healthPulsate, healthOffset, GetHitHealthColor());
			}
			else if (!NearDeathClock.Paused)
			{
				//else if the character is near-death, draw in near-death mode

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxHP, CurrentHP);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, currentAlpha, 1f, Vector2.One, Vector2.Zero, DepletedHealthColor);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, currentAlpha, Vector2.One, Vector2.Zero, GetNearDeathColor());
			}
			else
			{
				//otherwise draw the health bar normal

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxHP, CurrentHP);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, currentAlpha, 1f, Vector2.One, Vector2.Zero, DepletedHealthColor);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, currentAlpha, Vector2.One, Vector2.Zero, GetHealthColor());
			}
		}

		private Color GetHealthColor()
		{
			return LerpColors(HealthClock.CurrentTime * HealthColorSpeed, HealthColor);
		}

		private Color GetHitHealthColor()
		{
			if (!NearDeathClock.Paused)
			{
				var colors = new List<Color>(HitHealthColor);
				colors.AddRange(NearDeathColors);
				return LerpColors(HitTimer.CurrentTime * NearDeathColorSpeed, colors);
			}
			else
			{
				return LerpColors(HitTimer.CurrentTime * HitHealthColorSpeed, HitHealthColor);
			}
		}

		private Color GetHitDepletedHealthColor()
		{
			return LerpColors(HitTimer.CurrentTime * HitHealthDepletedColorSpeed, HitDepletedHealthColor);
		}

		private Color GetHealColor()
		{
			if (!NearDeathClock.Paused)
			{
				var colors = new List<Color>(HealColor);
				colors.AddRange(NearDeathColors);
				return LerpColors(HealTimer.CurrentTime * HealColorSpeed, colors);
			}
			else
			{
				return LerpColors(HealTimer.CurrentTime * HealColorSpeed, HealColor);
			}
		}

		private Color GetHealingColor()
		{
			return LerpColors(HealTimer.CurrentTime * HealColorSpeed, HealingColor);
		}

		private Color GetNearDeathColor()
		{
			return LerpColors(NearDeathClock.CurrentTime * NearDeathColorSpeed, NearDeathColors);
		}

		#endregion //Methods
	}
}
