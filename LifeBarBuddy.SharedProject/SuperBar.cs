using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBarBuddy
{
	public class SuperBar : Meter, ISuperBar
	{
		#region Properties

		private float _currentEnergy;
		public float CurrentEnergy
		{
			get
			{
				return _currentEnergy;
			}
			set
			{
				_currentEnergy = MathHelper.Clamp(value, 0f, MaxEnergy);
			}
		}

		private float _maxEnergy;
		public float MaxEnergy
		{
			get
			{
				return _maxEnergy;
			}
			set
			{
				if (value <= 0f)
				{
					throw new Exception($"MaxEnergy {value} is <= 0");
				}
				_maxEnergy = value;
			}
		}

		public List<Color> EnergyColor { get; set; }
		public Color EmptyEnergyColor { get; set; }
		public float EnergyColorSpeed { get; set; }
		public List<Color> AddEnergyColor { get; set; }
		public List<Color> AddingEnergyColor { get; set; }
		public float AddEnergyTimeDelta { get; set; }
		public float AddEnergyScaleAmount { get; set; }
		public float AddEnergyColorSpeed { get; set; }
		public float AddEnergyPulsateSpeed { get; set; }
		public List<Color> EnergyFullColor { get; set; }
		public float EnergyFullScaleAmount { get; set; }
		public float EnergyFullColorSpeed { get; set; }
		public float EnergyFullPulsateSpeed { get; set; }

		private GameClock EnergyClock { get; set; }

		private CountdownTimer AddEnergyTimer { get; set; }

		private GameClock EnergyFullClock { get; set; }

		private float PreAddAmount { get; set; }

		private bool IsFullEnergyMode
		{
			get
			{
				return CurrentEnergy >= MaxEnergy;
			}
		}

		#endregion //Properties

		#region Methods

		public SuperBar(float maxEnergy, ContentManager content, Filename borderImage, Filename meterImage, Filename alphaMaskImage)
		{
			EnergyClock = new GameClock();
			AddEnergyTimer = new CountdownTimer();
			EnergyFullClock = new GameClock();

			MaxEnergy = maxEnergy;
			CurrentEnergy = 0f;
			EnergyColor = new List<Color> { new Color(10, 130, 160), new Color(0, 0, 110) };
			EmptyEnergyColor = new Color(0.3f, 0.3f, 0.3f);
			EnergyColorSpeed = 0.75f;
			AddEnergyColor = new List<Color> { Color.DarkBlue, new Color(100,250,180) };
			AddingEnergyColor = new List<Color> { new Color(90, 180, 250), Color.PapayaWhip };
			AddEnergyTimeDelta = 1f;
			AddEnergyScaleAmount = 0.07f;
			AddEnergyColorSpeed = 2.5f;
			AddEnergyPulsateSpeed = 20f;
			EnergyFullColor = new List<Color> { Color.Blue, Color.LightBlue, new Color(0, 0, 80) };
			EnergyFullScaleAmount = 0.11f;
			EnergyFullColorSpeed = 8f;
			EnergyFullPulsateSpeed = 15f;

			LoadContent(content, borderImage, meterImage, alphaMaskImage);
		}

		public void Reset()
		{
			EnergyClock.Start();
			AddEnergyTimer.Stop();
			EnergyFullClock.Stop();
			CurrentEnergy = 0f;
		}

		public override void Update(GameClock time)
		{
			base.Update(time);

			//update all timers
			EnergyClock.Update(time);
			AddEnergyTimer.Update(time);
			EnergyFullClock.Update(time);

			//check if we need to change "full energy" mode
			if (EnergyFullClock.Paused && IsFullEnergyMode)
			{
				EnergyFullClock.Start();
			}
			else if (!EnergyFullClock.Paused && !IsFullEnergyMode)
			{
				EnergyFullClock.Stop();
			}
		}

		public void AddEnergy(float energy)
		{
			//is the player already being healed?
			if (!AddEnergyTimer.HasTimeRemaining)
			{
				PreAddAmount = CurrentEnergy;
			}
			else
			{
				PreAddAmount = PreAddAmount + ((CurrentEnergy - PreAddAmount) * (1 - AddEnergyTimer.Lerp));
			}

			//start the healing timer
			AddEnergyTimer.Start(AddEnergyTimeDelta);
		}

		public void Draw(float currentEnergy, IMeterRenderer meterRenderer, SpriteBatch spritebatch, Rectangle position)
		{
			CurrentEnergy = currentEnergy;

			if (AddEnergyTimer.HasTimeRemaining)
			{
				//If the character is being healed, draw in heal mode

				//how much health bar to draw?
				var healingHealthBar = PreAddAmount + ((CurrentEnergy - PreAddAmount) * (1f - AddEnergyTimer.Lerp));
				var healingAlpha = ConvertToAlpha(0, MaxEnergy, healingHealthBar);

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(AddEnergyTimer.CurrentTime * AddEnergyPulsateSpeed, AddEnergyScaleAmount, position);

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxEnergy, CurrentEnergy);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, 1f, Vector2.One, Vector2.Zero, EmptyEnergyColor);

				//draw the healing part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, healingAlpha, currentAlpha, Vector2.One, Vector2.Zero, GetAddingEnergyColor());

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, healingAlpha, Vector2.One, Vector2.Zero, GetAddEnergyColor());
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, healingAlpha, pulsate, Vector2.Zero, GetAddEnergyColor());
			}
			else if (!EnergyFullClock.Paused)
			{
				//else if the energy is full, draw in FullEnergy mode

				//how much pulsate to add to hp bar?
				var pulsate = PulsateScale(EnergyFullClock.CurrentTime * EnergyFullPulsateSpeed, EnergyFullScaleAmount, position);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, 1f, pulsate, Vector2.Zero, GetEnergyFullColor());
			}
			else
			{
				//otherwise draw the health bar normal

				//what is the alpha of the current HP?
				var currentAlpha = ConvertToAlpha(0, MaxEnergy, CurrentEnergy);

				//draw the empty part of the bar
				meterRenderer.DrawMeter(this, spritebatch, position, currentAlpha, 1f, Vector2.One, Vector2.Zero, EmptyEnergyColor);

				//draw the health bar
				meterRenderer.DrawMeter(this, spritebatch, position, 0f, currentAlpha, Vector2.One, Vector2.Zero, GetEnergyColor());
			}
		}

		private Color GetEnergyColor()
		{
			return LerpColors(EnergyClock.CurrentTime * EnergyColorSpeed, EnergyColor);
		}

		private Color GetAddEnergyColor()
		{
			if (!EnergyFullClock.Paused)
			{
				var colors = new List<Color>(AddEnergyColor);
				colors.AddRange(EnergyFullColor);
				return LerpColors(AddEnergyTimer.CurrentTime * EnergyFullColorSpeed, colors);
			}
			else
			{
				return LerpColors(AddEnergyTimer.CurrentTime * AddEnergyColorSpeed, AddEnergyColor);
			}
		}

		private Color GetAddingEnergyColor()
		{
			return LerpColors(AddEnergyTimer.CurrentTime * AddEnergyColorSpeed, AddingEnergyColor);
		}

		private Color GetEnergyFullColor()
		{
			return LerpColors(EnergyFullClock.CurrentTime * EnergyFullColorSpeed, EnergyFullColor);
		}

		#endregion //Methods
	}
}
