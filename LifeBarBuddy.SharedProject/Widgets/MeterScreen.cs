using LifeBarBuddy;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LifeBarBuddy
{
	public class MeterScreen : WidgetScreen
	{
		#region Properties

		public IMeter Meter { get; private set; }

		private Point MeterPosition { get; set; }

		private TransitionWipeType TransitionStyle { get; set; }

		private VerticalAlignment Vert { get; set; }

		private HorizontalAlignment Horiz { get; set; }

		#endregion //Properties

		#region Methods

		public MeterScreen(IMeter meter,
			Point meterPosition, 
			TransitionWipeType transitionStyle, 
			ContentManager content, 
			VerticalAlignment vert = VerticalAlignment.Top,
			HorizontalAlignment horiz = HorizontalAlignment.Center) : base("MeterScreen", content)
		{
			Meter = meter;
			MeterPosition = meterPosition;
			TransitionStyle = transitionStyle;
			Vert = vert;
			Horiz = horiz;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			//create the widget to hold the meter
			var meterShim = new MeterWidget(this, Meter)
			{
				TransitionObject = new WipeTransitionObject(TransitionStyle),
				Position = MeterPosition,
				Horizontal = Horiz,
				Vertical = Vert
			};
			AddItem(meterShim);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			//display the meter
			Meter.IsVisible = true;
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			//hide the meter
			Meter.IsVisible = false;
		}

		#endregion //Methods
	}
}
