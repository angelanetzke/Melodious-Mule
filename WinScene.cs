using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MelodiousMule
{
	internal class WinScene : AbstractScene
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly ContentManager Content;
		private SpriteFont font;
		private Texture2D buttonTexture;
		private Texture2D muleTexture;
		private readonly string[] winScreenText = new string[] {
			"You have found the legendary bugle! You triumphantly return\n",
			"home with your prize."
		};
		private readonly List<GameObject> allGameObjects = new();
		private readonly List<Button> buttons = new();
		private int textStartY = 0;
		private readonly int BUFFER = 10;

		public WinScene(GraphicsDeviceManager _graphics, ContentManager Content)
		{
			this._graphics = _graphics;
			this.Content = Content;
		}

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>(@"UI\Kenney_Pixel");
			buttonTexture = Content.Load<Texture2D>(@"UI\buttonshape");
			muleTexture = Content.Load<Texture2D>(@"UI\mule");
		}

		public override MelodiousMule.GameState Update(GameTime gameTime, MouseState mouseState, KeyboardState keyState)
		{
			foreach (Button thisButton in buttons)
			{
				var thisButtonClicked = thisButton.Update(mouseState);
				if (thisButtonClicked && thisButton.GetText() == "Play Again")
				{
					return MelodiousMule.GameState.PLAYING;
				}
				else if (thisButtonClicked && thisButton.GetText() == "How to Play")
				{
					return MelodiousMule.GameState.HELP_SCREEN;
				}
				else if (thisButtonClicked && thisButton.GetText() == "Exit")
				{
					return MelodiousMule.GameState.EXIT;
				}
			}
			return MelodiousMule.GameState.WIN;
		}
		public override void Draw(SpriteBatch batch)
		{
			if (buttons.Count == 0)
			{
				GenerateObjects();
			}
			batch.Begin();
			for (int i = 0; i < winScreenText.Length; i++)
			{
				batch.DrawString(
				font,
				winScreenText[i],
					new Vector2((_graphics.PreferredBackBufferWidth - font.MeasureString(winScreenText[i]).X) / 2,
						textStartY + font.MeasureString(winScreenText[i]).Y * i),
					Color.White);
			}
			batch.Draw(
			muleTexture,
			new Rectangle(
					(_graphics.PreferredBackBufferWidth - muleTexture.Width) / 2,
					textStartY + (int)font.MeasureString(winScreenText[0]).Y * winScreenText.Length + BUFFER,
					muleTexture.Width,
					muleTexture.Height),
					Color.White);
			foreach (GameObject thisGameObject in allGameObjects)
			{
				thisGameObject.Draw(batch);
			}
			batch.End();
		}

		private void GenerateObjects()
		{
			var allObjectsYSize =
				(int)font.MeasureString(winScreenText[0]).Y * winScreenText.Length + BUFFER 
				+ muleTexture.Height + BUFFER 
				+ buttonTexture.Height;
			textStartY = _graphics.PreferredBackBufferHeight / 2 - allObjectsYSize / 2;
			var centerX = _graphics.PreferredBackBufferWidth / 2;
			var buttonY = textStartY 
				+ (int)font.MeasureString(winScreenText[0]).Y * winScreenText.Length + BUFFER
				+ muleTexture.Height + BUFFER;
			var newButton = new Button(0, 0, "Play Again", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - 1.5f * newButton.GetSize().X - 10, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "How to Play", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - .5f * newButton.GetSize().X, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "Exit", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX + .5f * newButton.GetSize().X + 10, buttonY);
			buttons.Add(newButton);
			allGameObjects.AddRange(buttons);
		}

	}
}
