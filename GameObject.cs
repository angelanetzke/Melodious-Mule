using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RogueClone
{
	internal class GameObject
	{
		private Vector2 position;
		private Vector2 size;
		private Texture2D texture;

		public GameObject(float x, float y)
		{
			position = new Vector2(x, y);
		}

		public void SetPosition(float x, float y)
		{
			position = new Vector2(x, y);
		}

		public Vector2 GetPosition()
		{
			return position;
		}

		public void SetSize(int width, int height)
		{
			size = new Vector2(width, height);
		}

		public Vector2 GetSize()
		{
			return size;
		}

		public void SetTexture(Texture2D texture)
		{
			this.texture = texture;
		}

		public Texture2D GetTexture()
		{
			return texture;
		}

		public Rectangle GetRectangle()
		{
			return new Rectangle((int)Math.Round(GetPosition().X),
				(int)Math.Round(GetPosition().Y),
				(int)Math.Round(GetSize().X),
				(int)Math.Round(GetSize().X));
		}

		public virtual void Draw(SpriteBatch batch)
		{
			batch.Draw(texture, GetPosition(), Color.White);
		}


	}
}
