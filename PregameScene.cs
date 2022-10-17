﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MelodiousMule
{
	internal class PregameScene : AbstractScene
	{
		private readonly string[] startScreenText = new string[] {
			"All your life you've heard of the legend of the Melodious Mule,",
			"an extraordinary creature who could play a bugle. You always",
			"wondered whether the legend was true. You set out on a quest",
			"to find the bugle of the Melodious Mule, rumored to be on the",
			"10th level of some nearby caverns."
		};

		private readonly GraphicsDeviceManager _graphics;
		private readonly ContentManager Content;
		private readonly List<GameObject> allGameObjects = new();
		private readonly List<Button> buttons = new();
		private SpriteFont font;
		private Texture2D buttonTexture;

		public PregameScene(GraphicsDeviceManager _graphics, ContentManager Content)
		{
			this._graphics = _graphics;
			this.Content = Content;
		}

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>(@"UI\Kenney_Pixel");
			buttonTexture = Content.Load<Texture2D>(@"UI\buttonshape");
		}

		public override MelodiousMule.GameState Update(GameTime gameTime, MouseState mouseState, KeyboardState keyState)
		{
			foreach (Button thisButton in buttons)
			{
				var thisButtonClicked = thisButton.Update(mouseState);
				if (thisButtonClicked && thisButton.GetText() == "Play")
				{
					return MelodiousMule.GameState.PLAYING;
				}
				if (thisButtonClicked && thisButton.GetText() == "How to Play")
				{
					return MelodiousMule.GameState.HELP_SCREEN;
				}
			}
			return MelodiousMule.GameState.PREGAME;
		}

		public override void Draw(SpriteBatch batch)
		{
			if (buttons.Count == 0)
			{
				GenerateObjects();
			}
			batch.Begin();
			for (int i = 0; i < startScreenText.Length; i++)
			{
				batch.DrawString(
				font,
				startScreenText[i],
					new Vector2((_graphics.PreferredBackBufferWidth - font.MeasureString(startScreenText[i]).X) / 2,
						(font.MeasureString(startScreenText[i]).Y + 3) * i + 50),
					Color.White);
			}
			foreach (GameObject thisGameObject in allGameObjects)
			{
				thisGameObject.Draw(batch);
			}
			batch.End();
		}

		private void GenerateObjects()
		{
			var centerX = _graphics.PreferredBackBufferWidth / 2;
			var buttonY = (font.MeasureString(startScreenText[0]).Y + 3) * startScreenText.Length + 60;
			var newButton = new Button(0, 0, "Play", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - newButton.GetSize().X - 10, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "How to Play", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX + 10, buttonY);
			buttons.Add(newButton);
			allGameObjects.AddRange(buttons);
		}

	}
}