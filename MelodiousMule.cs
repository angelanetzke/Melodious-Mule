using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
		private List<Potion> potions = new();
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
		private Texture2D bugleTexture;
		private Texture2D muleTexture;
		private Texture2D potionHealthTexture;
		private Texture2D potionStrengthTexture;
		private Texture2D helpScreen01Texture;
		private Texture2D helpScreen02Texture;
		private Texture2D helpScreen03Texture;
		private Texture2D helpScreen04Texture;
		private Texture2D helpScreen05Texture;
		private Texture2D helpScreen06Texture;
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
		private readonly Bugle theBugle = new(0, 0);
		private readonly int CURSOR_SIZE = 32;
		private bool lastCanAttack = false;
		private enum GameState { PREGAME, PLAYING, WIN, LOSE, HELP_SCREEN };
		private GameState currentGameState;
		private int currentLevel = 0;
		private readonly int LEVEL_COUNT = 10;
		private readonly int[][] zombieDistribution = new int[][]
			{
				new int[] { 5, 0, 0},
				new int[] { 5, 2, 0},
				new int[] { 3, 5, 0},
				new int[] { 3, 7, 0},
				new int[] { 0, 10, 0},
				new int[] { 0, 14, 0},
				new int[] { 0, 12, 5},
				new int[] { 0, 12, 10},
				new int[] { 0, 12, 20},
				new int[] { 0, 0, 40}
			};
		private readonly string[] startScreenText = new string[] {
			"All your life you've heard of the legend of the Melodious Mule,",
			"an extraordinary creature who could play a bugle. You always",
			"wondered whether the legend was true. You set out on a quest",
			"to find the bugle of the Melodious Mule, rumored to be on the",
			"10th level of some nearby caverns."
		};
		private readonly string[] winScreenText = new string[] {
			"You have found the legendary bugle! You triumphantly return\n",
			"home with your prize."
		};
		private SpriteFont font;
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
				"Health and strength potions.",
				"There is one of each per level."
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
		private string loseScreenText = "You have died. The bugle remains unfound.";
		private List<int> roomNumbers = Enumerable.Range(0, 9).ToList();

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
			buttonTimer = 0f;
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
			bugleTexture = Content.Load<Texture2D>(@"Level\bugle");
			theBugle.SetTexture(bugleTexture);
			muleTexture = Content.Load<Texture2D>(@"UI\mule");
			potionHealthTexture = Content.Load<Texture2D>(@"Level\potion-health");
			potionStrengthTexture = Content.Load<Texture2D>(@"Level\potion-strength");
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
					var thisButtonClicked = thisButton.Update(mouseState);
					if (thisButtonClicked && thisButton.GetText() == "Play")
					{
						allGameObjects = new();
						currentGameState = GameState.PLAYING;
					}
					if (thisButtonClicked && thisButton.GetText() == "How to Play")
					{
						allGameObjects = new();
						currentGameState = GameState.HELP_SCREEN;
						GenerateHelpScreen();
					}
				}
			}
			else if (currentGameState == GameState.PLAYING)
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
				if (theHero.GetRectangle().Intersects(theBugle.GetRectangle()))
				{
					allGameObjects = new();
					currentGameState = GameState.WIN;
					Mouse.SetCursor(MouseCursor.Arrow);
					GenerateWinScreen();
				}
				if (currentLevel < LEVEL_COUNT - 1
					&& theHero.GetRectangle().Intersects(theStairs.GetRectangle()))
				{
					allGameObjects = new();
					currentLevel++;
				}
				if (theHero.GetHP() <= 0)
				{
					allGameObjects = new();
					currentGameState = GameState.LOSE;
					Mouse.SetCursor(MouseCursor.Arrow);
					GenerateLoseScreen();
				}
				CollectPotions();
				Attack(mouseState);
				foreach (Zombie thisZombie in zombies)
				{
					thisZombie.Update(theHero, walls.ConvertAll(x => (GameObject)x), gameTime);
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
			}
			else if (currentGameState == GameState.WIN)
			{
				foreach (Button thisButton in buttons)
				{
					var thisButtonClicked = thisButton.Update(mouseState);
					if (thisButtonClicked && thisButton.GetText() == "Play Again")
					{
						allGameObjects = new();
						currentGameState = GameState.PLAYING;
						currentLevel = 0;
						theHero.Reset();
					}
					else if (thisButtonClicked && thisButton.GetText() == "Exit")
					{
						Exit();
					}
				}
			}
			else if (currentGameState == GameState.HELP_SCREEN)
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
							allGameObjects = new();
							currentGameState = GameState.PLAYING;
							currentLevel = 0;
							theHero.Reset();
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
			}
			else if (currentGameState == GameState.LOSE)
			{
				foreach (Button thisButton in buttons)
				{
					var thisButtonClicked = thisButton.Update(mouseState);
					if (thisButtonClicked && thisButton.GetText() == "Play Again")
					{
						allGameObjects = new();
						currentGameState = GameState.PLAYING;
						currentLevel = 0;
						theHero.Reset();
					}
					else if (thisButtonClicked && thisButton.GetText() == "How to Play")
					{
						allGameObjects = new();
						currentGameState = GameState.HELP_SCREEN;
						GenerateHelpScreen();
					}
					else if (thisButtonClicked && thisButton.GetText() == "Exit")
					{
						Exit();
					}
				}
			}
			base.Update(gameTime);
		}

		private void CollectPotions()
		{
			var potionsToRemove = new List<Potion>();
			foreach (Potion thisPotion in potions)
			{
				if (theHero.GetRectangle().Intersects(thisPotion.GetRectangle()))
				{
					if (thisPotion.GetPotionType() == Potion.PotionType.HEALTH)
					{
						theHero.IncreaseHP();
					}
					if (thisPotion.GetPotionType() == Potion.PotionType.STRENGTH)
					{
						theHero.IncreaseStrength();
					}
					potionsToRemove.Add(thisPotion);
				}
			}
			foreach (Potion thisPotion in potionsToRemove)
			{
				potions.Remove(thisPotion);
				allGameObjects.Remove(thisPotion);
			}
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
			roomNumbers = roomNumbers.OrderBy(x => RNG.Next()).ToList();
			walls = new();
			zombies = new();
			allGameObjects.Add(theHero);
			ConnectMap();
			GenerateRooms();
			GenerateCorridors();
			allGameObjects.AddRange(walls);
			theHero.SetPosition(centerX[roomNumbers[0]] * TILE_SIZE, centerY[roomNumbers[0]] * TILE_SIZE);
			if (currentLevel < LEVEL_COUNT - 1)
			{
				var stairsPosition = rooms[roomNumbers[1]].GetRandomPointInside(4);
				theStairs.SetPosition(stairsPosition.X * TILE_SIZE, stairsPosition.Y * TILE_SIZE);
				allGameObjects.Add(theStairs);
			}
			else
			{
				var buglePosition = rooms[roomNumbers[1]].GetRandomPointInside(4);
				theBugle.SetPosition(buglePosition.X * TILE_SIZE, buglePosition.Y * TILE_SIZE);
				allGameObjects.Add(theBugle);
			}
			GeneratePotions();
			allGameObjects.AddRange(potions);
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
			for (int i = 0; i < zombieDistribution[currentLevel][0]; i++)
			{
				zombieLocation = rooms[roomNumbers[RNG.Next(1, roomNumbers.Count)]].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.EASY);
				zombie.SetTexture(zombieEasyTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel][1]; i++)
			{
				zombieLocation = rooms[roomNumbers[RNG.Next(1, roomNumbers.Count)]].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.MEDIUM);
				zombie.SetTexture(zombieMediumTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel][2]; i++)
			{
				zombieLocation = rooms[roomNumbers[RNG.Next(1, roomNumbers.Count)]].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.HARD);
				zombie.SetTexture(zombieHardTexture);
				zombies.Add(zombie);
			}
		}

		private void GeneratePotions()
		{
			potions = new();
			var potionPosition = rooms[roomNumbers[2]].GetRandomPointInside();
			var newPotion = new Potion(
				potionPosition.X * TILE_SIZE,
				potionPosition.Y * TILE_SIZE,
				Potion.PotionType.STRENGTH);
			newPotion.SetTexture(potionStrengthTexture);
			potions.Add(newPotion);
			potionPosition = rooms[roomNumbers[3]].GetRandomPointInside();
			newPotion = new Potion(
				potionPosition.X * TILE_SIZE,
				potionPosition.Y * TILE_SIZE,
				Potion.PotionType.HEALTH);
			newPotion.SetTexture(potionHealthTexture);
			potions.Add(newPotion);
		}

		private void GenerateStartScreen()
		{
			buttons = new();
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

		private void GenerateHelpScreen()
		{
			buttons = new();
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

		private void GenerateWinScreen()
		{
			buttons = new();
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

		private void GenerateLoseScreen()
		{
			buttons = new();
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
					+ $"   HP: {theHero.GetHP()}"
					+ $"   Strength: {theHero.GetStrength()}",
					new Vector2(10, 10),
					Color.White,
					0,
					Vector2.Zero,
					1,
					SpriteEffects.None,
					1);
				_spriteBatch.End();
			}
			else if (currentGameState == GameState.WIN)
			{
				_spriteBatch.Begin();
				for (int i = 0; i < winScreenText.Length; i++)
				{
					_spriteBatch.DrawString(
						font,
						winScreenText[i],
						new Vector2((_graphics.PreferredBackBufferWidth - font.MeasureString(winScreenText[i]).X) / 2,
							(font.MeasureString(winScreenText[i]).Y + 3) * i + 50),
						Color.White);
				}
				_spriteBatch.Draw(
					muleTexture,
					new Rectangle(
						(_graphics.PreferredBackBufferWidth - muleTexture.Width) / 2,
						(int)(font.MeasureString(winScreenText[0]).Y + 3) * winScreenText.Length + 60,
						muleTexture.Width,
						muleTexture.Height),
						Color.White);
				foreach (GameObject thisGameObject in allGameObjects)
				{
					thisGameObject.Draw(_spriteBatch);
				}
				_spriteBatch.End();
			}
			else if (currentGameState == GameState.HELP_SCREEN)
			{
				_spriteBatch.Begin();
				_spriteBatch.Draw(
					helpScreenIcons[helpScreenPage],
					new Vector2(
						_graphics.PreferredBackBufferWidth / 2
							- helpScreenIcons[helpScreenPage].Width / 2,
						10),
					Color.White);
				for (int i = 0; i < helpScreenText[helpScreenPage].Length; i++)
				{
					_spriteBatch.DrawString(
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
					buttons[i].Draw(_spriteBatch);
				}
				_spriteBatch.End();
			}
			else if (currentGameState == GameState.LOSE)
			{
				_spriteBatch.Begin();
				_spriteBatch.DrawString(
					font,
					loseScreenText,
					new Vector2(
						_graphics.PreferredBackBufferWidth / 2 - font.MeasureString(loseScreenText).X / 2,
						10),
					Color.White);
				foreach (Button thisButton in buttons)
				{
					thisButton.Draw(_spriteBatch);
				}
				_spriteBatch.End();
			}
			base.Draw(gameTime);
		}

	}
}