using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Zombie : GameObject
	{
		public enum Difficulty { EASY, MEDIUM, HARD };
		private readonly Difficulty difficulty;
		private readonly int[] SPEED = new int[] { 50, 100, 150 };
		private readonly int[] VISION = new int[] { 200, 250, 300 };
		
		public enum AnimType { MOVE_N, MOVE_S, MOVE_W, MOVE_E, IDLE };
		private AnimType currentAnimation = AnimType.IDLE;
		private Animation[] animations = new Animation[5];
		public Zombie(float x, float y, Difficulty difficulty) : base(x, y)
		{
			this.difficulty = difficulty;
			SetSize(30, 35);
			animations[(int)AnimType.MOVE_N] = new Animation(
				9 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_S] = new Animation(
				(int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_W] = new Animation(
				13 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_E] = new Animation(
				5 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.IDLE] = new Animation(0, 1, .2f, (int)Math.Round(GetSize().X));
		}

		public void Update(GameObject target, List<GameObject> obstacles, GameTime gameTime)
		{
			var movement = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED[(int)difficulty];
			SetCurrentAnimation(AnimType.IDLE);
			if (Vector2.Distance(target.GetPosition(), GetPosition()) < VISION[(int)difficulty])
			{
				var xDistance = 0;
				var yDistance = 0;
				var xAnimation = AnimType.IDLE;
				var yAnimation = AnimType.IDLE;
				if (target.GetPosition().X < GetPosition().X && CanMoveLeft(movement, obstacles))
				{
					xAnimation = AnimType.MOVE_W;
					xDistance = (int)(GetPosition().X - target.GetPosition().X);
					SetPosition(GetPosition().X - movement, GetPosition().Y);
				}
				if (target.GetPosition().X > GetPosition().X && CanMoveRight(movement, obstacles))
				{
					xAnimation = AnimType.MOVE_E;
					xDistance = (int)(target.GetPosition().X - GetPosition().X);
					SetPosition(GetPosition().X + movement, GetPosition().Y);
				}
				if (target.GetPosition().Y < GetPosition().Y && CanMoveUp(movement, obstacles))
				{
					yAnimation = AnimType.MOVE_N;
					yDistance = (int)(GetPosition().Y - target.GetPosition().Y);
					SetPosition(GetPosition().X, GetPosition().Y - movement);
				}
				if (target.GetPosition().Y > GetPosition().Y && CanMoveDown(movement, obstacles))
				{
					yAnimation = AnimType.MOVE_S;
					yDistance = (int)(target.GetPosition().Y - GetPosition().Y);
					SetPosition(GetPosition().X, GetPosition().Y + movement);
				}
				if (xDistance > yDistance)
				{
					SetCurrentAnimation(xAnimation);
				}
				else if (yDistance > xDistance)
				{
					SetCurrentAnimation(yAnimation);
				}
			}
			animations[(int)currentAnimation].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		public void SetCurrentAnimation(AnimType newAnimation)
		{
			currentAnimation = newAnimation;
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				animations[(int)currentAnimation].GetSourceRectangle(),
				Color.White,
				0,
				new Vector2(0, 0),
				SpriteEffects.None,
				.6f
			);
		}

	}
}
