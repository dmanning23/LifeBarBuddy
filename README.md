# LifeBarBuddy

This is a library for MonoGame projects for creating life bars, energy bars, & other types of meters.

![screenshot](/lifeBarBuddy.gif?raw=true "Optional Title")

How it works is create 3 images, the background, a mask for the meter, and an alpha gradient from 0-1. When the meter is drawn, all those assets and meter information are passed to a special shader that renders everything.

To use this library, install from the [Nuget package](https://www.nuget.org/packages/LifeBarBuddy/1.0.0)

Check out the example project at [LifeBarBuddySample](https://github.com/dmanning23/LifeBarBuddySample)

First create an instance of the type of meter you'd like. There are currently two built-in types: basic LifeBar and SuperBar (a single-stock super gauge similar to Super Street Fighter 2 Turbo)
```
public override void LoadContent()
		{
			base.LoadContent();

			//create the lifebar
			lifeBar = new LifeBar(maxHP, Content, "lifebarBorder.png", "lifebar.png", "lifebarGradient.png");
      
      //create the MeterRenderer object, which is used to do the rendering for all the meters in the game
      meterRenderer = new MeterRenderer(Content, new Filename("MeterShader.fx"));
      
      //Load the rest of your game assets...
    }
```

The object instance needs to be updated each frame with the current time:
```
public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			lifeBar.Update(gameTime);
      
      //Update the rest of your game objects...
    }
````

Call the correct methods to indicate to the meter when events occur:
```
private void Heal(object sender, EventArgs e)
		{
			var damage = 10f;
			hitPoints += damage;
			lifeBar.Heal(damage);
		}

		private void TakeDamage(object sender, EventArgs e)
		{
			var damage = 5f;
			hitPoints -= damage;
			lifeBar.AddDamage(damage);
		}
```

Lastly, the meter itself is stateless, so make sure you pass the correct current values in the Draw method:
```
public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
      
      //Draw all your game objects here...

			//draw the meters on top once everything else has finished rendering
			meterRenderer.SpriteBatchBegin(ScreenManager.SpriteBatch, Resolution.TransformationMatrix());
			lifeBar.Draw(hitPoints, meterRenderer, ScreenManager.SpriteBatch, lifebarRect);
			ScreenManager.SpriteBatch.End();
		}
```
