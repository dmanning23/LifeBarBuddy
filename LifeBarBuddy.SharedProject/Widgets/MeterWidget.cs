using GameTimer;
using MenuBuddy;
using Microsoft.Xna.Framework;
using System;

namespace LifeBarBuddy
{
	public class MeterWidget : Shim
	{
		#region Properties

		public IMeter Meter { get; private set; }
		private IScreen _screen;

		#endregion //Properties

		#region Methods

		public MeterWidget(IScreen screen, IMeter meter)
			: base(meter.Position.Width, meter.Position.Height)
		{
			_screen = screen;

			Meter = meter;
			SetMeterPosition();
		}

		public MeterWidget(Shim inst) : base(inst)
		{
			throw new NotImplementedException();
		}

		public override void Update(IScreen screen, GameClock gameTime)
		{
			base.Update(screen, gameTime);

			SetMeterPosition();
		}

		private void SetMeterPosition()
		{
			//update the meter position
			if (TransitionObject.ScreenTransition != null)
			{
				var position = TransitionObject.Position(Rect);
				Meter.Position = new Rectangle(position.X, position.Y, Meter.Position.Width, Meter.Position.Height);
			}
			else
			{
				Meter.IsVisible = false;
			}
		}

		#endregion //Methods
	}
}
