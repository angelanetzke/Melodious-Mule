using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MelodiousMule
{
	internal class Button : GameObject
	{
		private readonly Color backgroundNormal = Color.Gray;
		private readonly Color backgroundHover = Color.DarkGreen;
		private readonly Color textNormal = Color.Black;
		private readonly Color textHover = Color.White;
		private readonly SpriteFont font;
		private Color currentBackgroundColor;
		private Color currentTextColor;
		private readonly string text;

		public Button(float x, float y, string text, SpriteFont font) : base(x, y)
		{
			SetSize(200, 70);
			this.text = text;
			this.font = font;
			currentBackgroundColor = backgroundNormal;
			currentTextColor = textNormal;
		}

		public string GetText()
		{
			return text;
		}

		public bool Update(MouseState mouseState)
		{
			if (GetRectangle().Contains(mouseState.X, mouseState.Y) 
				&& currentBackgroundColor == backgroundNormal)
			{
				currentBackgroundColor = backgroundHover;
				currentTextColor= textHover;
			}
			else if (!GetRectangle().Contains(mouseState.X, mouseState.Y)
				&& currentBackgroundColor == backgroundHover)
			{
				currentBackgroundColor = backgroundNormal;
				currentTextColor = textNormal;
			}
			if (GetRectangle().Contains(mouseState.X, mouseState.Y)
				&& mouseState.LeftButton == ButtonState.Pressed)
			{
				return true;
			}
			return false;
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				currentBackgroundColor);
			batch.DrawString(
				font,
				text,
				new Vector2(GetPosition().X + (GetSize().X - font.MeasureString(text).X) / 2,
					GetPosition().Y + (GetSize().Y - font.MeasureString(text).Y) / 2),
				currentTextColor,
				0,
				Vector2.Zero,
				1,
				SpriteEffects.None,
				1);
		}

	}
}
