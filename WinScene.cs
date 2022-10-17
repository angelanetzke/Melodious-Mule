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
		private readonly PlayingScene thePlayingScene;

		public WinScene(GraphicsDeviceManager _graphics, ContentManager Content, PlayingScene thePlayingScene)
		{
			this._graphics = _graphics;
			this.Content = Content;
			this.thePlayingScene = thePlayingScene;			
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
						(font.MeasureString(winScreenText[i]).Y + 3) * i + 50),
					Color.White);
			}
			batch.Draw(
			muleTexture,
			new Rectangle(
					(_graphics.PreferredBackBufferWidth - muleTexture.Width) / 2,
					(int)(font.MeasureString(winScreenText[0]).Y + 3) * winScreenText.Length + 60,
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
			var centerX = _graphics.PreferredBackBufferWidth / 2;
			var buttonY = (font.MeasureString(winScreenText[0]).Y + 3) * winScreenText.Length
				+ 60 + muleTexture.Height + 10;
			var newButton = new Button(0, 0, "Play Again", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - newButton.GetSize().X - 10, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "Exit", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX + 10, buttonY);
			buttons.Add(newButton);
			allGameObjects.AddRange(buttons);
		}

	}
}
