using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MelodiousMule
{
	internal class HelpScene : AbstractScene
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly ContentManager Content;
		private readonly List<Button> buttons = new();
		private SpriteFont font;
		private Texture2D buttonTexture;
		private Texture2D helpScreen01Texture;
		private Texture2D helpScreen02Texture;
		private Texture2D helpScreen03Texture;
		private Texture2D helpScreen04Texture;
		private Texture2D helpScreen05Texture;
		private Texture2D helpScreen06Texture;
		private readonly string[][] helpScreenText = new string[][]
		{
			new string[] {"You, the hero. Move with WASD." },
			new string[] {"The bugle. Find this to win the game!" },
			new string[]
			{
				"Easy, medium, and hard zombies.",
				"They will reduce your HP when you come in contact with them."
			},
			new string[]
			{
				"Your targeting reticle, indicating when you can or can't attack",
				"based on distance and whether walls are in the way.",
				"Control with mouse."
			},
			new string[]
			{
				"Health and strength potions. There is one of each per level.",
				"Health potions increase your current and maximum HP. HP increases over time.",
				"Strength potions increase your attack power."
			},
			new string[]
			{
				"The stairs. Use to travel to the next level.",
				"Once you leave a level, there is no going back."
			}
		};
		private List<Texture2D> helpScreenIcons = new();
		private int helpScreenPage = 0;
		private readonly float BUTTON_COOLDOWN = .25f;
		private float buttonTimer;

		public HelpScene(GraphicsDeviceManager _graphics, ContentManager Content)
		{
			this._graphics = _graphics;
			this.Content = Content;
			buttonTimer = 0f;			
		}
		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>(@"UI\Kenney_Pixel");
			buttonTexture = Content.Load<Texture2D>(@"UI\buttonshape");
			helpScreen01Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen01");
			helpScreen02Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen02");
			helpScreen03Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen03");
			helpScreen04Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen04");
			helpScreen05Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen05");
			helpScreen06Texture = Content.Load<Texture2D>(@"HelpScreen\helpscreen06");
			helpScreenIcons.Add(helpScreen01Texture);
			helpScreenIcons.Add(helpScreen02Texture);
			helpScreenIcons.Add(helpScreen03Texture);
			helpScreenIcons.Add(helpScreen04Texture);
			helpScreenIcons.Add(helpScreen05Texture);
			helpScreenIcons.Add(helpScreen06Texture);
		}
		public override MelodiousMule.GameState Update(GameTime gameTime, MouseState mouseState, KeyboardState keyState)
		{
			buttonTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (buttonTimer > BUTTON_COOLDOWN)
			{
				foreach (Button thisButton in buttons)
				{
					var thisButtonClicked = thisButton.Update(mouseState);
					if (thisButtonClicked)
					{
						buttonTimer = 0f;
					}
					if (thisButtonClicked && thisButton.GetText() == "Play")
					{
						return MelodiousMule.GameState.PLAYING;
					}
					else if (thisButtonClicked && thisButton.GetText() == "Previous")
					{
						helpScreenPage--;
						if (helpScreenPage < 0)
						{
							helpScreenPage = 0;
						}
					}
					else if (thisButtonClicked && thisButton.GetText() == "Next")
					{
						helpScreenPage++;
						if (helpScreenPage >= helpScreenIcons.Count)
						{
							helpScreenPage = helpScreenIcons.Count - 1;
						}
					}
				}
			}
			return MelodiousMule.GameState.HELP_SCREEN;
		}
		public override void Draw(SpriteBatch batch)
		{
			if (buttons.Count == 0)
			{
				GenerateObjects();
			}
			batch.Begin();
			batch.Draw(
				helpScreenIcons[helpScreenPage],
				new Vector2(
					_graphics.PreferredBackBufferWidth / 2
						- helpScreenIcons[helpScreenPage].Width / 2,
					10),
				Color.White);
			for (int i = 0; i < helpScreenText[helpScreenPage].Length; i++)
			{
				batch.DrawString(
					font,
					helpScreenText[helpScreenPage][i],
					new Vector2(
						_graphics.PreferredBackBufferWidth / 2
							- font.MeasureString(helpScreenText[helpScreenPage][i]).X / 2,
						font.MeasureString(helpScreenText[helpScreenPage][i]).Y * i + 74),
					Color.White);
			}
			for (int i = 0; i < buttons.Count; i++)
			{
				if (helpScreenPage == 0 && i == 0)
				{
					continue;
				}
				if (helpScreenPage == helpScreenIcons.Count - 1 && i == buttons.Count - 1)
				{
					continue;
				}
				buttons[i].Draw(batch);
			}
			batch.End();
		}
		private void GenerateObjects()
		{
			var centerX = _graphics.PreferredBackBufferWidth / 2;
			var buttonY = font.MeasureString(helpScreenText[0][0]).Y * 4 + 74;
			var newButton = new Button(0, 0, "Previous", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - 1.5f * newButton.GetSize().X - 10, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "Play", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX - .5f * newButton.GetSize().X, buttonY);
			buttons.Add(newButton);
			newButton = new Button(0, 0, "Next", font);
			newButton.SetTexture(buttonTexture);
			newButton.SetPosition(centerX + .5f * newButton.GetSize().X + 10, buttonY);
			buttons.Add(newButton);
		}


	}
}
