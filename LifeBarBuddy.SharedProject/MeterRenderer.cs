using FilenameBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LifeBarBuddy
{
	public class MeterRenderer : IMeterRenderer
	{
		#region Properties

		/// <summary>
		/// Shader to draw the texture, light correctly using the supplied normal map
		/// </summary>
		private Effect _meterEffect;

		private EffectParameterCollection _effectsParams;

		#endregion //Properties

		#region Methods

		public MeterRenderer(ContentManager content, Filename shaderFile)
		{
			_meterEffect = content.Load<Effect>(shaderFile.GetRelPathFileNoExt());
			_effectsParams = _meterEffect.Parameters;
		}

		public void SpriteBatchBegin(SpriteBatch spritebatch, Matrix translation)
		{
			spritebatch.Begin(SpriteSortMode.Immediate,
				BlendState.NonPremultiplied,
				null,
				null,
				RasterizerState.CullNone,
				_meterEffect,
				translation);
		}

		public void DrawMeter(IMeter meter, SpriteBatch spritebatch, Rectangle rect, float start, float end, Vector2 scale, Vector2 offset, Color color)
		{
			if (start == end)
			{
				return;
			}

			_effectsParams["AlphaMaskTexture"].SetValue(meter.AlphaMaskImage);
			_effectsParams["Start"].SetValue(start);
			_effectsParams["End"].SetValue(end);

			//update the position by adding the scale and offset
			var scaleOffset = new Vector2(((rect.Width - (rect.Width * scale.X)) * .5f),
				((rect.Height - (rect.Height * scale.Y)) * .5f));

			rect.X = (int)(scaleOffset.X + (rect.X + offset.X));
			rect.Y = (int)(scaleOffset.Y + (rect.Y + offset.Y));
			rect.Width = (int)(rect.Width * scale.X);
			rect.Height = (int)(rect.Height * scale.Y);

			spritebatch.Draw(meter.MeterImage, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

		}

		#endregion //Methods
	}
}
