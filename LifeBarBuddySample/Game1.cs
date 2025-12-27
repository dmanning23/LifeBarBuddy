using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LifeBarBuddySample
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
#if ANDROID || __IOS__
	public class Game1 : TouchGame
#else
	public class Game1 : MouseGame
#endif
	{
		public Game1()
		{
			IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
#if !__IOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		public override IScreen[] GetMainMenuScreenStack()
		{
			return new IScreen[] { new MainPage() };
		}
	}
}
