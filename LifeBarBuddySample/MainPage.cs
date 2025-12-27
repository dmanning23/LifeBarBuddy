using GameTimer;
using LifeBarBuddy;
using MenuBuddy;
using Microsoft.Xna.Framework;
using ResolutionBuddy;
using System;
using System.Threading.Tasks;

namespace LifeBarBuddySample
{
	public class MainPage : WidgetScreen
	{
		#region Properties

		const float maxHP = 100f;
		const float maxSuper = 100f;
		const float maxTime = 6f;
		const float maxMana = 10f;

		float hitPoints;
		float superEnergy;
		float mana;

		ILifeBar lifeBar;
		ISuperBar superBar;
		ITimerMeter timer;
		IManaBar manaBar;

		IMeterRenderer meterRenderer;

		CountdownTimer time;

		#endregion //Properties

		#region Methods

		public MainPage() : base("MainPage")
		{
			Transition.OnTime = 3f;
		}

		private void Reset()
		{
			hitPoints = maxHP;
			superEnergy = 0;
			mana = 0;
			lifeBar.Reset();
			superBar.Reset();
			manaBar.Reset();
			timer.Reset();
			time = new CountdownTimer();
		}

		public override async Task LoadContent()
		{
			await base.LoadContent();

			//create the meter rectangles
			var lifebarRect = new Rectangle(
				(int)Resolution.TitleSafeArea.Left,
				(int)Resolution.TitleSafeArea.Top,
				512, 128);

			var superBarRect = new Rectangle(
				(int)Resolution.TitleSafeArea.Left,
				lifebarRect.Bottom,
				512, 128);

			var manaBarRect = new Rectangle(
				(int)Resolution.TitleSafeArea.Left,
				superBarRect.Bottom,
				1024, 256);

			var timerRect = new Rectangle(600,
				(int)Resolution.TitleSafeArea.Top,
				128, 128);

			//create the lifebar
			lifeBar = new LifeBar(maxHP, Content, "lifebarborder.png", "lifebar.png", "lifebarGradient.png", lifebarRect);
			timer = new TimerMeter(maxTime, Content, "TimerBackground.png", "TimerMeter.png", "TimerGradient.png", timerRect);
			manaBar = new ManaBar(maxMana, Content, "energybackground.png", "energymeter.png", "energygradient.png", manaBarRect);
			superBar = new SuperBar(maxSuper, Content, "EnergyMeter1.png", "EnergyMeterMask1.png", "EnergyMeterGradient1.png", superBarRect);
			meterRenderer = new MeterRenderer(Content, "MeterShader.fx");

			//add a stack of buttons for interacting with stuff
			var lifeButtonStack = new StackLayout(StackAlignment.Bottom)
			{
				Position = new Point(Resolution.TitleSafeArea.Right, Resolution.TitleSafeArea.Bottom),
				Horizontal = HorizontalAlignment.Right,
				Vertical = VerticalAlignment.Top
			};
			var hitButton = AddButton("Add Damage");
			hitButton.OnClick += HitButton_OnClick;
			lifeButtonStack.AddItem(hitButton);

			var healButton = AddButton("Heal");
			healButton.OnClick += HealButton_OnClick;
			lifeButtonStack.AddItem(healButton);

			var addSuperButton = AddButton("Add Energy");
			addSuperButton.OnClick += AddSuperButton_OnClick;
			lifeButtonStack.AddItem(addSuperButton);

			var spendSuperButton = AddButton("Use Super");
			spendSuperButton.OnClick += SpendSuperButton_OnClick;
			lifeButtonStack.AddItem(spendSuperButton);

			var addManaButton = AddButton("Add Mana");
			addManaButton.OnClick += AddManaButton_OnClick;
			lifeButtonStack.AddItem(addManaButton);

			var spendManaButton = AddButton("Use Mana");
			spendManaButton.OnClick += SpendManaButton_OnClick;
			lifeButtonStack.AddItem(spendManaButton);

			var nopeButton = AddButton("Super Nope");
			nopeButton.OnClick += NopeButton_OnClick;
			lifeButtonStack.AddItem(nopeButton);

			var resetTime = AddButton("ResetTime");
			resetTime.OnClick += ResetTime_OnClick;
			lifeButtonStack.AddItem(resetTime);

			AddItem(lifeButtonStack);

			Reset();
		}

		private void ResetTime_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			time.Start(maxTime);
			timer.Reset();
		}

		private void NopeButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			superBar.Nope();
		}

		private void SpendSuperButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			superEnergy = 0;
			superBar.UseEnergy();
		}

		private void AddSuperButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 10f;
			superEnergy += damage;
			superBar.AddEnergy(damage);
		}

		private void SpendManaButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var spellCost = 2f;
			mana = Math.Max(0f, Math.Min(mana - spellCost, maxMana));
			manaBar.UseMana(spellCost);
		}

		private void AddManaButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			mana = Math.Max(0f, Math.Min(mana + 0.5f, maxMana));
		}

		private void HealButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 10f;
			hitPoints += damage;
			lifeBar.Heal(damage);
		}

		private void HitButton_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			var damage = 5f;
			hitPoints -= damage;
			lifeBar.AddDamage(damage);
		}

		private RelativeLayoutButton AddButton(string text)
		{
			var hitButton = new RelativeLayoutButton()
			{
				Size = new Vector2(256, 64),
				HasOutline = true,
				HasBackground = true,
				Horizontal = HorizontalAlignment.Left,
				Vertical = VerticalAlignment.Top
			};
			hitButton.AddItem(new Label(text, Content, FontSize.Small)
			{
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Center
			});
			return hitButton;
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			time.Update(gameTime);

			lifeBar.Update(gameTime);
			superBar.Update(gameTime);
			manaBar.Update(gameTime);
			timer.Update(time);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//draw the meters
			meterRenderer.Alpha = Transition.Alpha;
			meterRenderer.SpriteBatchBegin(ScreenManager.SpriteBatch, Resolution.TransformationMatrix());

			lifeBar.Draw(hitPoints, meterRenderer, ScreenManager.SpriteBatch, false);

			superBar.Draw(superEnergy, meterRenderer, ScreenManager.SpriteBatch, false);

			manaBar.Draw(mana, meterRenderer, ScreenManager.SpriteBatch, false);

			timer.Draw(time.RemainingTime, meterRenderer, ScreenManager.SpriteBatch);

			ScreenManager.SpriteBatch.End();
		}

		#endregion //Methods
	}
}
