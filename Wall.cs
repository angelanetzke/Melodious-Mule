using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Wall : GameObject
	{
		public enum WallType { CORNER_NW, CORNER_NE, CORNER_SW, CORNER_SE, WALL_H, WALL_V };
		private readonly WallType wallType;
		private readonly float rotation;
		public Wall(float x, float y) : base(x, y)
		{
			SetSize(16, 16);
		}

		public Wall(float x, float y, WallType wallType) : this(x, y)
		{
			this.wallType = wallType;
			switch (wallType)
			{
				case WallType.CORNER_NW:
					rotation = 0;
					break;
				case WallType.CORNER_NE:
					rotation = MathHelper.ToRadians(90);
					break;
				case WallType.CORNER_SE:
					rotation = MathHelper.ToRadians(180);
					break;
				case WallType.CORNER_SW:
					rotation = MathHelper.ToRadians(270);
					break;
				case WallType.WALL_H:
					rotation = 0;
					break;
				case WallType.WALL_V:
					rotation = MathHelper.ToRadians(90);
					break;
			}
		}

		public WallType GetWallType()
		{
			return wallType;
		}
		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(
				GetTexture(),
				new Rectangle((int)(GetPosition().X + GetSize().X / 2), 
					(int)(GetPosition().Y + GetSize().Y / 2),
					(int)GetSize().X, (int)GetSize().Y),
				null,
				Color.White,
				rotation,
				new Vector2(GetSize().X / 2, GetSize().Y / 2),
				SpriteEffects.None,
				0);
		}

	}
}
