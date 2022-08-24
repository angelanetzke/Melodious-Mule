using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Stairs : GameObject
	{
		public Stairs (float x, float y) : base(x, y)
		{
			SetSize(32, 32);
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
				.7f
			);
		}
	}
}
