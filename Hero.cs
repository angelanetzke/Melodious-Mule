using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RogueClone
{
	internal class Hero : GameObject
	{
		public enum AnimType { MOVE_N, MOVE_S, MOVE_W, MOVE_E, IDLE };
		private AnimType currentAnimation = AnimType.IDLE;
		private Animation[] animations = new Animation[5];
		private readonly int SPEED = 200;
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

		public Vector2 Update(KeyboardState keyState, List<GameObject> obstacles, GameTime gameTime,
			float xTranslation, float yTranslation)
		{
			SetCurrentAnimation(Hero.AnimType.IDLE);
			float movement = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
			if (keyState.IsKeyDown(Keys.A) && CanMoveLeft(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_W);
				SetPosition(GetPosition().X - movement, GetPosition().Y);
				xTranslation += movement;
			}
			if (keyState.IsKeyDown(Keys.D) && CanMoveRight(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_E);
				SetPosition(GetPosition().X + movement, GetPosition().Y);
				xTranslation -= movement;
			}
			if (keyState.IsKeyDown(Keys.W) && CanMoveUp(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_N);
				SetPosition(GetPosition().X, GetPosition().Y - movement);
				yTranslation += movement;
			}
			if (keyState.IsKeyDown(Keys.S) && CanMoveDown(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_S);
				SetPosition(GetPosition().X, GetPosition().Y + movement);
				yTranslation -= movement;
			}
			animations[(int)currentAnimation].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			return new Vector2(xTranslation, yTranslation);
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
				1
			);
		}

	}
}
