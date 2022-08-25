using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Potion : GameObject
	{
		public enum PotionType { HEALTH, STRENGTH };
		private readonly PotionType type;
		public Potion(float x, float y, PotionType type) : base(x, y)
		{
			SetSize(16, 16);
			this.type = type;
		}
		public PotionType GetPotionType()
		{
			return type;
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
