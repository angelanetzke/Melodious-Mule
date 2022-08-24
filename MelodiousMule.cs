using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MelodiousMule
{
	public class MelodiousMule : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private List<GameObject> allGameObjects = new();
		private List<Wall> walls = new();
		private List<Zombie> zombies = new();
		private List<Button> buttons = new();
		private readonly Random RNG = new();
		private Texture2D cornerTexture;
		private Texture2D wallTexture;
		private Texture2D heroTexture;
		private Texture2D crosshairGreenTexture;
		private Texture2D crosshairRedTexture;
		private Texture2D zombieEasyTexture;
		private Texture2D zombieMediumTexture;
		private Texture2D zombieHardTexture;
		private Texture2D groundTexture;
		private Texture2D buttonTexture;
		private Texture2D stairsTexture;
		private readonly int TILE_SIZE = 16;
		private readonly int[] centerX = new int[] { 20, 70, 120, 20, 70, 120, 20, 70, 120 };
		private readonly int[] centerY = new int[] { 20, 20, 20, 70, 70, 70, 120, 120, 120 };
		private readonly int minRoomSize = 5;
		private readonly int maxRoomSize = 20;
		private readonly int corridorWidth = 3;
		private float xTranslation;
		private float yTranslation;
		private readonly List<Room> rooms = new();
		private readonly int ROOM_COUNT = 9;
		private readonly Hero theHero = new(0, 0);
		private readonly Stairs theStairs = new(0, 0);
		private int heroStartRoom = 0;
		private readonly int CURSOR_SIZE = 32;
		bool lastCanAttack = false;
		private enum GameState { PREGAME, PLAYING };
		private GameState currentGameState;
		private int currentLevel = 0;
		private readonly int LEVEL_COUNT = 10;
		private readonly int[][] zombieDistribution = new int[][]
			{
				new int[] { 3, 3, 3},
				new int[] { 3, 1, 0},
				new int[] { 3, 2, 0},
				new int[] { 3, 3, 0},
				new int[] { 0, 5, 0},
				new int[] { 0, 6, 0},
				new int[] { 0, 7, 0},
				new int[] { 0, 5, 2},
				new int[] { 0, 5, 3},
				new int[] { 0, 0, 8}
			};
		private readonly string[] startScreenText = new string[] {
			"All your life you've heard of the legend of the Melodious Mule,",
			"an extraordinary creature who could play a bugle. You always",
			"wondered whether the legend was true. You set out on a quest",
			"to find the bugle of the Melodious Mule, rumored to be on the",
			"10th level of some nearby caverns." };
		private SpriteFont font;

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
			for (int i = 0; i < ROOM_COUNT; i++)
			{
				rooms.Add(new Room());
			}
			rooms[0].SetNeighbor(rooms[1], Room.Direction.E);
			rooms[0].SetNeighbor(rooms[3], Room.Direction.S);
			rooms[1].SetNeighbor(rooms[0], Room.Direction.W);
			rooms[1].SetNeighbor(rooms[4], Room.Direction.S);
			rooms[1].SetNeighbor(rooms[2], Room.Direction.E);
			rooms[2].SetNeighbor(rooms[1], Room.Direction.W);
			rooms[2].SetNeighbor(rooms[5], Room.Direction.S);
			rooms[3].SetNeighbor(rooms[0], Room.Direction.N);
			rooms[3].SetNeighbor(rooms[4], Room.Direction.E);
			rooms[3].SetNeighbor(rooms[6], Room.Direction.S);
			rooms[4].SetNeighbor(rooms[1], Room.Direction.N);
			rooms[4].SetNeighbor(rooms[3], Room.Direction.W);
			rooms[4].SetNeighbor(rooms[5], Room.Direction.E);
			rooms[4].SetNeighbor(rooms[7], Room.Direction.S);
			rooms[5].SetNeighbor(rooms[2], Room.Direction.N);
			rooms[5].SetNeighbor(rooms[4], Room.Direction.W);
			rooms[5].SetNeighbor(rooms[8], Room.Direction.S);
			rooms[6].SetNeighbor(rooms[3], Room.Direction.N);
			rooms[6].SetNeighbor(rooms[7], Room.Direction.E);
			rooms[7].SetNeighbor(rooms[4], Room.Direction.N);
			rooms[7].SetNeighbor(rooms[6], Room.Direction.W);
			rooms[7].SetNeighbor(rooms[8], Room.Direction.E);
			rooms[8].SetNeighbor(rooms[5], Room.Direction.N);
			rooms[8].SetNeighbor(rooms[7], Room.Direction.W);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			cornerTexture = Content.Load<Texture2D>(@"Level\corner");
			wallTexture = Content.Load<Texture2D>(@"Level\wall");
			heroTexture = Content.Load<Texture2D>(@"Level\hero");
			theHero.SetTexture(heroTexture);
			crosshairGreenTexture = Content.Load<Texture2D>(@"Level\crosshair-green");
			crosshairRedTexture = Content.Load<Texture2D>(@"Level\crosshair-red");
			zombieEasyTexture = Content.Load<Texture2D>(@"Level\zombie-easy");
			zombieMediumTexture = Content.Load<Texture2D>(@"Level\zombie-medium");
			zombieHardTexture = Content.Load<Texture2D>(@"Level\zombie-hard");
			groundTexture = Content.Load<Texture2D>(@"Level\ground");
			font = Content.Load<SpriteFont>(@"UI\Kenney_Pixel");
			buttonTexture = Content.Load<Texture2D>(@"UI\buttonshape");
			stairsTexture = Content.Load<Texture2D>(@"Level\stairs");
			theStairs.SetTexture(stairsTexture);
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			var keyState = Keyboard.GetState();
			var mouseState = Mouse.GetState();
			if (currentGameState == GameState.PREGAME)
			{
				if (allGameObjects.Count == 0)
				{
					GenerateStartScreen();
				}
				foreach (Button thisButton in buttons)
				{
					if (thisButton.Update(mouseState) && thisButton.GetText() == "Play")
					{
						allGameObjects = new();
						currentGameState = GameState.PLAYING;
					}
				}
			}
			if (currentGameState == GameState.PLAYING)
			{
				if (allGameObjects.Count == 0)
				{
					GenerateMap();
					var transformMatrix = Matrix.Invert(Matrix.CreateTranslation(xTranslation, yTranslation, 0));
					var worldMousePosition =
						Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), transformMatrix);
					var mouseX = worldMousePosition.X;
					var mouseY = worldMousePosition.Y;
					var canAttack = theHero.CanAttack(walls.ConvertAll(x => (GameObject)x), mouseX, mouseY);
					if (canAttack)
					{
						lastCanAttack = true;
						Mouse.SetCursor(MouseCursor.FromTexture2D(crosshairGreenTexture, CURSOR_SIZE, CURSOR_SIZE));
					}
					else
					{
						lastCanAttack = false;
						Mouse.SetCursor(MouseCursor.FromTexture2D(crosshairRedTexture, CURSOR_SIZE, CURSOR_SIZE));
					}
				}
				var newTranslation =
					theHero.Update(
						keyState,
						walls.ConvertAll(x => (GameObject)x),
						gameTime,
						xTranslation,
						yTranslation);
				xTranslation = newTranslation.X;
				yTranslation = newTranslation.Y;
				Attack(mouseState);
				foreach (Zombie thisZombie in zombies)
				{
					thisZombie.Update(theHero, walls.ConvertAll(x => (GameObject)x), gameTime);
				}
				if (theHero.GetRectangle().Intersects(theStairs.GetRectangle()))
				{
					allGameObjects = new();
					currentLevel++;
					if (currentLevel == LEVEL_COUNT)
					{
						Exit();
					}
				}
			}
			base.Update(gameTime);
		}

		private void Attack(MouseState mouseState)
		{
			var transformMatrix = Matrix.Invert(Matrix.CreateTranslation(xTranslation, yTranslation, 0));
			var worldMousePosition =
				Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), transformMatrix);
			var mouseX = worldMousePosition.X;
			var mouseY = worldMousePosition.Y;
			bool canAttack = theHero.CanAttack(walls.ConvertAll(x => (GameObject)x), mouseX, mouseY);
			if (canAttack && !lastCanAttack)
			{
				Mouse.SetCursor(MouseCursor.FromTexture2D(crosshairGreenTexture, CURSOR_SIZE, CURSOR_SIZE));
			}
			else if (!canAttack && lastCanAttack)
			{
				Mouse.SetCursor(MouseCursor.FromTexture2D(crosshairRedTexture, CURSOR_SIZE, CURSOR_SIZE));
			}
			lastCanAttack = canAttack;
			if (mouseState.LeftButton == ButtonState.Pressed && canAttack)
			{
				var objectsToRemove = theHero.Attack(zombies, mouseX, mouseY);
				foreach (Zombie thisObject in objectsToRemove)
				{
					allGameObjects.Remove(thisObject);
					zombies.Remove(thisObject);
				}
			}
		}

		private void GenerateMap()
		{
			walls = new();
			zombies = new();
			allGameObjects.Add(theHero);
			ConnectMap();
			GenerateRooms();
			GenerateCorridors();
			allGameObjects.AddRange(walls);
			var roomSelect = RNG.Next(rooms.Count);
			theHero.SetPosition(centerX[roomSelect] * TILE_SIZE, centerY[roomSelect] * TILE_SIZE);
			heroStartRoom = roomSelect;
			while (roomSelect == heroStartRoom)
			{
				roomSelect = RNG.Next(rooms.Count);
			}
			if (currentLevel < LEVEL_COUNT - 1)
			{
				var stairsPosition = rooms[roomSelect].GetRandomPointInside(3);
				theStairs.SetPosition(stairsPosition.X * TILE_SIZE, stairsPosition.Y * TILE_SIZE);
				allGameObjects.Add(theStairs);
			}
			GenerateZombies();
			allGameObjects.AddRange(zombies);
			xTranslation = _graphics.PreferredBackBufferWidth / 2 - theHero.GetPosition().X - theHero.GetSize().X / 2;
			yTranslation = _graphics.PreferredBackBufferHeight / 2 - theHero.GetPosition().Y - theHero.GetSize().Y / 2;
		}

		private void ConnectMap()
		{
			foreach (Room thisRoom in rooms)
			{
				thisRoom.Reset();
			}
			var selectedRoom = rooms[RNG.Next(rooms.Count)];
			selectedRoom.SetIsVisited(true);
			while (rooms.Count(x => x.IsVisited()) < rooms.Count)
			{
				if (selectedRoom == null)
				{
					var alreadyConnected = rooms.Where(x => x.IsVisited()).ToList();
					selectedRoom = alreadyConnected[RNG.Next(alreadyConnected.Count)];
				}
				selectedRoom = selectedRoom.ConnectToRandom();
			}
		}

		private void GenerateRooms()
		{
			for (int i = 0; i < centerX.Length; i++)
			{
				var minX = centerX[i] - RNG.Next(minRoomSize, maxRoomSize);
				var maxX = centerX[i] + RNG.Next(minRoomSize, maxRoomSize);
				var minY = centerY[i] - RNG.Next(minRoomSize, maxRoomSize);
				var maxY = centerY[i] + RNG.Next(minRoomSize, maxRoomSize);
				rooms[i].SetDimensions(minX, maxX, minY, maxY, centerX[i], centerY[i]);
				GenerateCorners(rooms[i]);
				GenerateHorizontalWalls(rooms[i]);
				GenerateVerticalWalls(rooms[i]);
				GameObject ground;
				for (int thisX = minX + 1; thisX < maxX; thisX++)
				{
					for (int thisY = minY + 1; thisY < maxY; thisY++)
					{
						ground = new GameObject(thisX * TILE_SIZE, thisY * TILE_SIZE);
						ground.SetTexture(groundTexture);
						allGameObjects.Add(ground);
					}
				}
			}
		}

		private void GenerateCorners(Room theRoom)
		{
			int minX = theRoom.GetMinX();
			int maxX = theRoom.GetMaxX();
			int minY = theRoom.GetMinY();
			int maxY = theRoom.GetMaxY();
			var wallPiece = new Wall(minX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_NW);
			wallPiece.SetTexture(cornerTexture);
			walls.Add(wallPiece);
			wallPiece = new Wall(minX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_SW);
			wallPiece.SetTexture(cornerTexture);
			walls.Add(wallPiece);
			wallPiece = new Wall(maxX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_NE);
			wallPiece.SetTexture(cornerTexture);
			walls.Add(wallPiece);
			wallPiece = new Wall(maxX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_SE);
			wallPiece.SetTexture(cornerTexture);
			walls.Add(wallPiece);
		}

		private void GenerateHorizontalWalls(Room theRoom)
		{
			int minX = theRoom.GetMinX();
			int maxX = theRoom.GetMaxX();
			int minY = theRoom.GetMinY();
			int maxY = theRoom.GetMaxY();
			int centerX = theRoom.GetCenterX();
			Wall wallPiece;
			GameObject ground;
			for (int thisX = minX + 1; thisX < maxX; thisX++)
			{
				if (theRoom.IsConnected(Room.Direction.N) && Math.Abs(thisX - centerX) <= corridorWidth)
				{
					if (thisX == centerX - corridorWidth)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_SE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisX == centerX + corridorWidth)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_SW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else
					{
						ground = new GameObject(thisX * TILE_SIZE, minY * TILE_SIZE);
						ground.SetTexture(groundTexture);
						allGameObjects.Add(ground);
					}
				}
				else
				{
					wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.WALL_H);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
				if (theRoom.IsConnected(Room.Direction.S) && Math.Abs(thisX - centerX) <= corridorWidth)
				{
					if (thisX == centerX - corridorWidth)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_NE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisX == centerX + corridorWidth)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_NW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else
					{
						ground = new GameObject(thisX * TILE_SIZE, maxY * TILE_SIZE);
						ground.SetTexture(groundTexture);
						allGameObjects.Add(ground);
					}
				}
				else
				{
					wallPiece = new Wall(thisX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.WALL_H);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
			}
		}

		private void GenerateVerticalWalls(Room theRoom)
		{
			int minX = theRoom.GetMinX();
			int maxX = theRoom.GetMaxX();
			int minY = theRoom.GetMinY();
			int maxY = theRoom.GetMaxY();
			int centerY = theRoom.GetCenterY();
			Wall wallPiece;
			GameObject ground;
			for (int thisY = minY + 1; thisY < maxY; thisY++)
			{
				if (theRoom.IsConnected(Room.Direction.W) && Math.Abs(thisY - centerY) <= corridorWidth)
				{
					if (thisY == centerY - corridorWidth)
					{
						wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_SE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisY == centerY + corridorWidth)
					{
						wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_NE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else
					{
						ground = new GameObject(minX * TILE_SIZE, thisY * TILE_SIZE);
						ground.SetTexture(groundTexture);
						allGameObjects.Add(ground);
					}
				}
				else
				{
					wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.WALL_V);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
				if (theRoom.IsConnected(Room.Direction.E) && Math.Abs(thisY - centerY) <= corridorWidth)
				{
					if (thisY == centerY - corridorWidth)
					{
						wallPiece = new Wall(maxX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_SW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisY == centerY + corridorWidth)
					{
						wallPiece = new Wall(maxX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_NW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else
					{
						ground = new GameObject(maxX * TILE_SIZE, thisY * TILE_SIZE);
						ground.SetTexture(groundTexture);
						allGameObjects.Add(ground);
					}
				}
				else
				{
					wallPiece = new Wall(maxX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.WALL_V);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
			}
		}

		private void GenerateCorridors()
		{
			Wall wallPiece;
			GameObject ground;
			for (int i = 0; i < rooms.Count; i++)
			{
				if ((i % 3 == 0 || i % 3 == 1) && rooms[i].IsConnected(Room.Direction.E))
				{
					int startX = rooms[i].GetMaxX() + 1;
					int endX = rooms[i + 1].GetMinX() - 1;
					for (int x = startX; x <= endX; x++)
					{
						wallPiece = new Wall(x * TILE_SIZE, (centerY[i] - corridorWidth) * TILE_SIZE, Wall.WallType.WALL_H);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						wallPiece = new Wall(x * TILE_SIZE, (centerY[i] + corridorWidth) * TILE_SIZE, Wall.WallType.WALL_H);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						for (int y = centerY[i] - corridorWidth + 1; y < centerY[i] + corridorWidth; y++)
						{
							ground = new GameObject(x * TILE_SIZE, y * TILE_SIZE);
							ground.SetTexture(groundTexture);
							allGameObjects.Add(ground);
						}
					}
				}
				if ((i / 3 == 0 || i / 3 == 1) && rooms[i].IsConnected(Room.Direction.S))
				{
					int startY = rooms[i].GetMaxY() + 1;
					int endY = rooms[i + 3].GetMinY() - 1;
					for (int y = startY; y <= endY; y++)
					{
						wallPiece = new Wall((centerX[i] - corridorWidth) * TILE_SIZE, y * TILE_SIZE, Wall.WallType.WALL_V);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						wallPiece = new Wall((centerX[i] + corridorWidth) * TILE_SIZE, y * TILE_SIZE, Wall.WallType.WALL_V);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						for (int x = centerX[i] - corridorWidth + 1; x < centerX[i] + corridorWidth; x++)
						{
							ground = new GameObject(x * TILE_SIZE, y * TILE_SIZE);
							ground.SetTexture(groundTexture);
							allGameObjects.Add(ground);
						}
					}
				}
			}
		}

		private void GenerateZombies()
		{
			Zombie zombie;
			Point zombieLocation;
			int roomSelect;
			for (int i = 0; i < zombieDistribution[currentLevel][0]; i++)
			{
				roomSelect = heroStartRoom;
				while (roomSelect == heroStartRoom)
				{
					roomSelect = RNG.Next(rooms.Count);
				}
				zombieLocation = rooms[roomSelect].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.EASY);
				zombie.SetTexture(zombieEasyTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel][1]; i++)
			{
				roomSelect = heroStartRoom;
				while (roomSelect == heroStartRoom)
				{
					roomSelect = RNG.Next(rooms.Count);
				}
				zombieLocation = rooms[roomSelect].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.MEDIUM);
				zombie.SetTexture(zombieMediumTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel][2]; i++)
			{
				roomSelect = heroStartRoom;
				while (roomSelect == heroStartRoom)
				{
					roomSelect = RNG.Next(rooms.Count);
				}
				zombieLocation = rooms[roomSelect].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.HARD);
				zombie.SetTexture(zombieHardTexture);
				zombies.Add(zombie);
			}
		}

		private void GenerateStartScreen()
		{
			buttons = new();
			var startButton = new Button(100, 400, "Play", font);
			startButton.SetTexture(buttonTexture);
			buttons.Add(startButton);
			allGameObjects.Add(startButton);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			if (currentGameState == GameState.PREGAME)
			{
				_spriteBatch.Begin();
				for (int i = 0; i < startScreenText.Length; i++)
				{
					_spriteBatch.DrawString(
						font,
						startScreenText[i],
						new Vector2((_graphics.PreferredBackBufferWidth - font.MeasureString(startScreenText[i]).X) / 2,
							(font.MeasureString(startScreenText[i]).Y + 3) * i + 50),
						Color.White);
				}
				foreach (GameObject thisGameObject in allGameObjects)
				{
					thisGameObject.Draw(_spriteBatch);
				}
				_spriteBatch.End();
			}
			else if (currentGameState == GameState.PLAYING)
			{
				_spriteBatch.Begin(
					sortMode: SpriteSortMode.FrontToBack,
					transformMatrix: Matrix.CreateTranslation(xTranslation, yTranslation, 0));
				foreach (GameObject thisGameObject in allGameObjects)
				{
					thisGameObject.Draw(_spriteBatch);
				}
				_spriteBatch.End();
				_spriteBatch.Begin();
				_spriteBatch.DrawString(
					font,
					$"Level: {currentLevel + 1}"
						+ $"   HP: {theHero.GetHP()}",
					new Vector2(10, 10),
					Color.White,
					0,
					Vector2.Zero,
					1,
					SpriteEffects.None,
					1);
				_spriteBatch.End();
			}
			base.Draw(gameTime);
		}

	}
}