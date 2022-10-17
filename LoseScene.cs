using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MelodiousMule
{
	internal class LoseScene : AbstractScene
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly ContentManager Content;
		private SpriteFont font;
		private Texture2D buttonTexture;
		private readonly string loseScreenText = "You have died. The bugle remains unfound.";
		private readonly List<Button> buttons = new();
		private readonly PlayingScene thePlayingScene;

		public LoseScene(GraphicsDeviceManager _graphics, ContentManager Content, PlayingScene thePlayingScene)
		{
			this._graphics = _graphics;
			this.Content = Content;
			this.thePlayingScene = thePlayingScene;
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
			return MelodiousMule.GameState.LOSE;
		}
		public override void Draw(SpriteBatch batch)
		{
			if (buttons.Count == 0)
			{
				GenerateObjects();
			}
			batch.Begin();
			batch.DrawString(
			font,
			loseScreenText,
			new Vector2(
					_graphics.PreferredBackBufferWidth / 2 - font.MeasureString(loseScreenText).X / 2,
					10),
				Color.White);
			foreach (Button thisButton in buttons)
			{
				thisButton.Draw(batch);
			}
			batch.End();
		}

		private void GenerateObjects()
		{
			var centerX = _graphics.PreferredBackBufferWidth / 2;
			var buttonY = font.MeasureString(loseScreenText).Y + 15;
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
		}

	}
}
