using FilenameBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LifeBarBuddy
{
	public interface IMeterRenderer
    {
		void SpriteBatchBegin(SpriteBatch spritebatch, Matrix translation);

		/// <summary>
		/// Draw the border image around the meter
		/// </summary>
		/// <param name="start">The value to start drawing the meter, between 0f -> 1f</param>
		/// <param name="end">The value to stop drawing the meter, between 0f -> 1f & must be greater that start</param>
		/// <param name="scale">Amount to scale the meter</param>
		/// <param name="offset">Pixel offset to draw the meter</param>
		/// <param name="color">The color to draw the meter.</param>
		void DrawBorder(IMeter meter, SpriteBatch spritebatch, Rectangle rect, Vector2 scale, Vector2 offset, Color color);

		/// <summary>
		/// Draw the meter!
		/// </summary>
		/// <param name="start">The value to start drawing the meter, between 0f -> 1f</param>
		/// <param name="end">The value to stop drawing the meter, between 0f -> 1f & must be greater that start</param>
		/// <param name="scale">Amount to scale the meter</param>
		/// <param name="offset">Pixel offset to draw the meter</param>
		/// <param name="color">The color to draw the meter.</param>
		void DrawMeter(IMeter meter, SpriteBatch spritebatch, Rectangle rect, float start, float end, Vector2 scale, Vector2 offset, Color color);
	}
}
