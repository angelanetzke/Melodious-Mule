using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Bugle : GameObject
	{
		public Bugle(float x, float y) : base(x, y)
		{
			SetSize(48, 24);
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				null,
				Color.White,
				0,
				new Vector2(0, 0),
				SpriteEffects.None,
				.6f
			);
		}
	}
}
