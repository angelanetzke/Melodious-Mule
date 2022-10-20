using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MelodiousMule
{
	internal class PlayingScene : AbstractScene
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly ContentManager Content;
		private List<GameObject> allGameObjects = new();
		private List<Wall> walls = new();
		private List<Zombie> zombies = new();
		private List<Potion> potions = new();
		private readonly Random RNG = new();
		private List<int> roomNumbers = Enumerable.Range(0, 9).ToList();
		private SpriteFont font;
		private Texture2D cornerTexture;
		private Texture2D wallTexture;
		private Texture2D heroTexture;
		private Texture2D crosshairGreenTexture;
		private Texture2D crosshairRedTexture;
		private Texture2D zombieEasyTexture;
		private Texture2D zombieMediumTexture;
		private Texture2D zombieHardTexture;
		private Texture2D groundTexture;
		private Texture2D stairsTexture;
		private Texture2D bugleTexture;
		private Texture2D potionHealthTexture;
		private Texture2D potionStrengthTexture;
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
		private readonly Hero theHero;
		private readonly Stairs theStairs = new(0, 0);
		private readonly Bugle theBugle = new(0, 0);
		private readonly int CURSOR_SIZE = 32;
		private bool lastCanAttack = false;
		private int currentLevel = 1;
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

		public PlayingScene(GraphicsDeviceManager _graphics, ContentManager Content)
		{
			this._graphics = _graphics;
			this.Content = Content;
			theHero = new(0, 0, this.Content);
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
		}

		public override void LoadContent()
		{
			font = Content.Load<SpriteFont>(@"UI\Kenney_Pixel");
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
			stairsTexture = Content.Load<Texture2D>(@"Level\stairs");
			theStairs.SetTexture(stairsTexture);
			bugleTexture = Content.Load<Texture2D>(@"Level\bugle");
			theBugle.SetTexture(bugleTexture);
			potionHealthTexture = Content.Load<Texture2D>(@"Level\potion-health");
			potionStrengthTexture = Content.Load<Texture2D>(@"Level\potion-strength");
		}

		public override MelodiousMule.GameState Update(GameTime gameTime, MouseState mouseState, KeyboardState keyState)
		{
			if (currentLevel == LEVEL_COUNT 
				&& theHero.GetRectangle().Intersects(theBugle.GetRectangle()))
			{
				Reset();
				Mouse.SetCursor(MouseCursor.Arrow);
				return MelodiousMule.GameState.WIN;
			}
			if (currentLevel < LEVEL_COUNT
				&& theHero.GetRectangle().Intersects(theStairs.GetRectangle()))
			{
				currentLevel++;
				GenerateMap();
			}
			if (theHero.GetHP() <= 0)
			{
				Reset();
				Mouse.SetCursor(MouseCursor.Arrow);
				return MelodiousMule.GameState.LOSE;
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
			return MelodiousMule.GameState.PLAYING;
		}

		public override void Draw(SpriteBatch batch)
		{
			if (allGameObjects.Count == 0)
			{
				Reset();
			}
			batch.Begin(
					sortMode: SpriteSortMode.FrontToBack,
					transformMatrix: Matrix.CreateTranslation(xTranslation, yTranslation, 0));
			foreach (GameObject thisGameObject in allGameObjects)
			{
				thisGameObject.Draw(batch);
			}
			batch.End();
			batch.Begin();
			batch.DrawString(
				font,
				$"Level: {currentLevel}"
				+ $"   HP: {theHero.GetHP()} ({theHero.GetMaxHP()})"
				+ $"   Strength: {theHero.GetStrength()}",
				new Vector2(10, 10),
				Color.White,
				0,
				Vector2.Zero,
				1,
				SpriteEffects.None,
				1);
			batch.End();
		}

		private void GenerateMap()
		{
			allGameObjects = new() { theHero };
			roomNumbers = roomNumbers.OrderBy(x => RNG.Next()).ToList();
			walls = new();
			zombies = new();
			ConnectMap();
			GenerateRooms();
			GenerateCorridors();
			allGameObjects.AddRange(walls);
			theHero.SetPosition(centerX[roomNumbers[0]] * TILE_SIZE, centerY[roomNumbers[0]] * TILE_SIZE);
			if (currentLevel < LEVEL_COUNT)
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

		private void InitializeMouse()
		{
			var mouseState = Mouse.GetState();
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
			for (int i = 0; i < zombieDistribution[currentLevel - 1][0]; i++)
			{
				zombieLocation = rooms[roomNumbers[RNG.Next(1, roomNumbers.Count)]].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.EASY);
				zombie.SetTexture(zombieEasyTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel - 1][1]; i++)
			{
				zombieLocation = rooms[roomNumbers[RNG.Next(1, roomNumbers.Count)]].GetRandomPointInside(3);
				zombie = new Zombie(zombieLocation.X * TILE_SIZE, zombieLocation.Y * TILE_SIZE,
					Zombie.Difficulty.MEDIUM);
				zombie.SetTexture(zombieMediumTexture);
				zombies.Add(zombie);
			}
			for (int i = 0; i < zombieDistribution[currentLevel - 1][2]; i++)
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

		public void Reset()
		{
			theHero.Reset();
			currentLevel = 1;
			GenerateMap();
			InitializeMouse();
		}

	}
}
