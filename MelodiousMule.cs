using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MelodiousMule
{
	public class MelodiousMule : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		public enum GameState { PREGAME, PLAYING, WIN, LOSE, HELP_SCREEN, EXIT };
		private GameState currentGameState;
		private GameState lastGameState;
		private readonly List<AbstractScene> scenes = new();
		private Song themeSong;
		private Song loseSong;
		private bool hasSongStarted = false;

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
			currentGameState = GameState.PREGAME;
			lastGameState = GameState.PREGAME;
			scenes.Add(new PregameScene(_graphics, Content));
			scenes.Add(new PlayingScene(_graphics, Content));
			scenes.Add(new WinScene(_graphics, Content));
			scenes.Add(new LoseScene(_graphics, Content));
			scenes.Add(new HelpScene(_graphics, Content));
			MediaPlayer.Volume = .45f;
			base.Initialize();
		}

		protected override void LoadContent()
		{
			foreach (AbstractScene thisScene in scenes)
			{
				thisScene.LoadContent();
			}
			themeSong = Content.Load<Song>(@"Audio\song");
			loseSong = Content.Load<Song>(@"Audio\lose");
			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			if (!hasSongStarted)
			{
				switch (currentGameState)
				{
					case GameState.PREGAME:
					case GameState.WIN:
						MediaPlayer.Play(themeSong);
						break;
					case GameState.LOSE:
						MediaPlayer.Play(loseSong);
						break;
					default:
						MediaPlayer.Stop();
						break;
				}
				hasSongStarted = true;
			}
			var keyState = Keyboard.GetState();
			var mouseState = Mouse.GetState();
			if (currentGameState == GameState.EXIT)
			{
				Exit();
			}
			else
			{
				lastGameState = currentGameState;
				currentGameState = scenes[(int)currentGameState].Update(gameTime, mouseState, keyState);
				if (lastGameState != currentGameState)
				{
					hasSongStarted = false;
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