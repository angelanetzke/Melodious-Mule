using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace MelodiousMule
{
	public class MelodiousMule : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		public enum GameState { PREGAME, PLAYING, WIN, LOSE, HELP_SCREEN, EXIT };
		private GameState currentGameState;
		private readonly List<AbstractScene> scenes = new();

		public MelodiousMule()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_graphics.IsFullScreen = false;
			_graphics.PreferredBackBufferWidth =
				GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 50;
			_graphics.PreferredBackBufferHeight =
				GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
			_graphics.ApplyChanges();
		}

		protected override void Initialize()
		{
			currentGameState = GameState.LOSE;
			scenes.Add(new PregameScene(_graphics, Content));
			scenes.Add(new PlayingScene(_graphics, Content));
			scenes.Add(new WinScene(_graphics, Content));
			scenes.Add(new LoseScene(_graphics, Content));
			scenes.Add(new HelpScene(_graphics, Content));	
			base.Initialize();
		}

		protected override void LoadContent()
		{
			foreach (AbstractScene thisScene in scenes)
			{
				thisScene.LoadContent();
			}
			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
			else
			{
				var keyState = Keyboard.GetState();
				var mouseState = Mouse.GetState();
				if (currentGameState == GameState.EXIT)
				{
					Exit();
				}
				else
				{
					currentGameState = scenes[(int)currentGameState].Update(gameTime, mouseState, keyState);
				}
			}
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			if (currentGameState != GameState.EXIT)
			{
				scenes[(int)currentGameState].Draw(_spriteBatch);
			}
			base.Draw(gameTime);
		}

	}
}