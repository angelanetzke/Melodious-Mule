using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueClone
{
	internal class Hero : GameObject
	{
		public enum AnimType { MOVE_N, MOVE_S, MOVE_W, MOVE_E, IDLE };
		private AnimType currentAnimation = AnimType.IDLE;
		private Animation[] animations = new Animation[5];
		public Hero(float x, float y) : base(x, y)
		{
			SetSize(32, 32);
			animations[(int)AnimType.MOVE_N] = new Animation(5 * 32, 4, .2f, 32);
			animations[(int)AnimType.MOVE_S] = new Animation(0, 4, .2f, 32);
			animations[(int)AnimType.MOVE_W] = new Animation(10 * 32, 4, .2f, 32);
			animations[(int)AnimType.MOVE_E] = new Animation(10 * 32, 4, .2f, 32);
			animations[(int)AnimType.IDLE] = new Animation(4 * 32, 1, .2f, 32);
		}

		public void SetCurrentAnimation(AnimType newAnimation)
		{
			currentAnimation = newAnimation;
		}

		public void Update(float time)
		{
			animations[(int)currentAnimation].Update(time);
		}

		public override void Draw(SpriteBatch batch)
		{
			SpriteEffects effects;
			if (currentAnimation == AnimType.MOVE_W)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
			else
			{
				effects = SpriteEffects.None;
			}
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				animations[(int)currentAnimation].GetSourceRectangle(),
				Color.White,
				0,
				new Vector2(0, 0),
				effects,
				0
			);
		}
	}
}
