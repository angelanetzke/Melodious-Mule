using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueClone
{
	public class Rogue : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private List<GameObject> allGameObjects;
		private List<Wall> walls;
		private Random RNG;
		private Texture2D cornerTexture;
		private Texture2D wallTexture;
		private readonly int TILE_SIZE = 16;
		private readonly int[] centerX = new int[] { 20, 70, 120, 20, 70, 120, 20, 70, 120 };
		private readonly int[] centerY = new int[] { 20, 20, 20, 70, 70, 70, 120, 120, 120 };
		private readonly int minSize = 5;
		private readonly int maxSize = 20;
		private float xTranslation;
		private float yTranslation;
		private readonly float SPEED = 200;
		private readonly List<Room> rooms = new();
		private readonly int ROOM_COUNT = 9;

		public Rogue()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_graphics.IsFullScreen = false;
			_graphics.PreferredBackBufferWidth = 1000;
			_graphics.PreferredBackBufferHeight = 600;
			_graphics.ApplyChanges();
		}

		protected override void Initialize()
		{
			RNG = new Random();
			allGameObjects = new();
			walls = new();
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
			cornerTexture = Content.Load<Texture2D>("corner");
			wallTexture = Content.Load<Texture2D>("wall");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			var keyState = Keyboard.GetState();
			if (keyState.IsKeyDown(Keys.M))
			{
				GenerateMap();
			}
			float movement = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
			if (keyState.IsKeyDown(Keys.A))
			{
				xTranslation += movement;
			}
			if (keyState.IsKeyDown(Keys.D))
			{
				xTranslation -= movement;
			}
			if (keyState.IsKeyDown(Keys.W))
			{
				yTranslation += movement;
			}
			if (keyState.IsKeyDown(Keys.S))
			{
				yTranslation -= movement;
			}


			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(transformMatrix:
				Matrix.CreateTranslation(xTranslation, yTranslation, 0));
			foreach (GameObject thisAbstractGameObject in allGameObjects)
			{
				thisAbstractGameObject.Draw(_spriteBatch);
			}
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private void GenerateMap()
		{
			allGameObjects = new();
			walls = new();
			ConnectMap();
			GenerateRooms();
			GenerateCorridors();
			allGameObjects.AddRange(walls);
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
				var minX = centerX[i] - RNG.Next(minSize, maxSize);
				var maxX = centerX[i] + RNG.Next(minSize, maxSize);
				var minY = centerY[i] - RNG.Next(minSize, maxSize);
				var maxY = centerY[i] + RNG.Next(minSize, maxSize);
				rooms[i].SetDimensions(minX, maxX, minY, maxY, centerX[i], centerY[i]);
				GenerateCorners(rooms[i]);
				GenerateHorizontalWalls(rooms[i]);
				GenerateVerticalWalls(rooms[i]);
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
			for (int thisX = minX + 1; thisX < maxX; thisX++)
			{
				if (theRoom.IsConnected(Room.Direction.N) && Math.Abs(thisX - centerX) <= 2)
				{
					if (thisX == centerX - 2)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_SE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisX == centerX + 2)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.CORNER_SW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
				}
				else
				{
					wallPiece = new Wall(thisX * TILE_SIZE, minY * TILE_SIZE, Wall.WallType.WALL_H);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
				if (theRoom.IsConnected(Room.Direction.S) && Math.Abs(thisX - centerX) <= 2)
				{
					if (thisX == centerX - 2)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_NE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisX == centerX + 2)
					{
						wallPiece = new Wall(thisX * TILE_SIZE, maxY * TILE_SIZE, Wall.WallType.CORNER_NW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
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
			for (int thisY = minY + 1; thisY < maxY; thisY++)
			{
				if (theRoom.IsConnected(Room.Direction.W) && Math.Abs(thisY - centerY) <= 2)
				{
					if (thisY == centerY - 2)
					{
						wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_SE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisY == centerY + 2)
					{
						wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_NE);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
				}
				else
				{
					wallPiece = new Wall(minX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.WALL_V);
					wallPiece.SetTexture(wallTexture);
					walls.Add(wallPiece);
				}
				if (theRoom.IsConnected(Room.Direction.E) && Math.Abs(thisY - centerY) <= 2)
				{
					if (thisY == centerY - 2)
					{
						wallPiece = new Wall(maxX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_SW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
					}
					else if (thisY == centerY + 2)
					{
						wallPiece = new Wall(maxX * TILE_SIZE, thisY * TILE_SIZE, Wall.WallType.CORNER_NW);
						wallPiece.SetTexture(cornerTexture);
						walls.Add(wallPiece);
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
			for (int i = 0; i < rooms.Count; i++)
			{
				if ((i % 3 == 0 || i % 3 == 1) && rooms[i].IsConnected(Room.Direction.E))
				{
					int startX = rooms[i].GetMaxX() + 1;
					int endX = rooms[i + 1].GetMinX() - 1;
					for (int x = startX; x <= endX; x++)
					{
						wallPiece = new Wall(x * TILE_SIZE, (centerY[i] - 2) * TILE_SIZE, Wall.WallType.WALL_H);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						wallPiece = new Wall(x * TILE_SIZE, (centerY[i] + 2) * TILE_SIZE, Wall.WallType.WALL_H);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
					}
				}
				if ((i / 3 == 0 || i / 3 == 1) && rooms[i].IsConnected(Room.Direction.S))
				{
					int startY = rooms[i].GetMaxY() + 1;
					int endY = rooms[i + 3].GetMinY() - 1;
					for (int y = startY; y <= endY; y++)
					{
						wallPiece = new Wall((centerX[i] - 2) * TILE_SIZE, y * TILE_SIZE, Wall.WallType.WALL_V);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
						wallPiece = new Wall((centerX[i] + 2) * TILE_SIZE, y * TILE_SIZE, Wall.WallType.WALL_V);
						wallPiece.SetTexture(wallTexture);
						walls.Add(wallPiece);
					}
				}
			}
			
		}

	}
}